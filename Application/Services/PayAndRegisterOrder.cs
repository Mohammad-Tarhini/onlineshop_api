using onlineshopowner_api.Application.Dtos;
using onlineshopowner_api.Application.Dtos.Payment;
using onlineshopowner_api.Application.Dtos.PaymentAndOrder;
using onlineshopowner_api.Application.Interfaces.Iservices;
using onlineshopowner_api.Domain.Entities.PaymentAndOrder;
using onlineshopowner_api.Domain.Interfaces.IExternalServices;
using onlineshopowner_api.Infrastructure.ExternalServices.Payment;
using onlineshopowner_api.Infrastructure.OnException;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace onlineshopowner_api.Application.Services
{
    public class PayAndRegisterOrder : IOrderServices
    {

        private readonly IUserContextService usercontext;
        private readonly IUnityOfWork unityOfWork;
        private readonly IGoogleMapService googleMapService;
        private readonly ITwilioMessageService twilioMessageService;
        private readonly IEmailService emailservice;
        private IFakeGatewayService fakeGatewayService;

        public PayAndRegisterOrder(IUnityOfWork unityOfWork, IUserContextService userContext, IGoogleMapService googleMapService, ITwilioMessageService twilioMessageService, IFakeGatewayService fakeGatewayService, IEmailService emailservice)
        {
            this.unityOfWork = unityOfWork;
            this.googleMapService = googleMapService;
            usercontext = userContext;
            this.twilioMessageService = twilioMessageService;
            this.fakeGatewayService = fakeGatewayService;
            this.emailservice = emailservice;
        }
        //+++++++++++++++++++++++++++++++++++new++++++++++++++++++++++++++++++++++++++++++++++++
        public async Task<(string sessionId, string checkoutUrl)> registerOrder(PayDto dto)
        {
            int userid = usercontext.GetUserId();
            string role = usercontext.GetUserRole();
            if (role.ToLower() != "client")
            {
                throw new Exception("user is not client");
            }
            var client =await unityOfWork.PersonRepository.GetClientIdByPersonId(userid);
            if (client == null) 
            {
                throw new DomainException("");
            }
            var (products, totalPriceOfProducts) = await CheckExistenceOfOrderItem(dto.Items, dto.OrderLocation.shopid);
            var deliveryCost = await calculateDeliveryPrice(dto.DeliveryProviderId, dto.OrderLocation.shopid, dto.OrderLocation.latitude, dto.OrderLocation.longitude);

            var clientorder = new Domain.Entities.PaymentAndOrder.clientOrder
            {
                clientId = client.Value,
                DeliveryCost = deliveryCost,
                latitude = dto.OrderLocation.latitude,
                longitude = dto.OrderLocation.longitude,
                shopId = dto.OrderLocation.shopid,
                ProductTotalCost = totalPriceOfProducts,
                deliveryProviderId = dto.DeliveryProviderId,

                // totalcost = totalPriceOfProducts + deliveryPrice
            };
            decimal totalCost = totalPriceOfProducts + deliveryCost;
            var orderItems = new List<Domain.Entities.PaymentAndOrder.OrderDetail>();
            foreach (var item in products)
            {
                var orderItem = new Domain.Entities.PaymentAndOrder.OrderDetail
                {
                    productId = item.productId,
                    quantity = item.quantity,
                };
                orderItems.Add(orderItem);

            }

            var orderId = await unityOfWork.paymentAndOrderRepository.RegisterOrder(clientorder, orderItems);


            var paymentRequest = new CheckoutSessionRequest
            {
                OrderId = orderId,
                Amount = totalCost,
                WebhookUrl = "https://localhost:44364/api/gateway/webhook"
            };
            var paymentResult = await fakeGatewayService.CreateSessionAsync(paymentRequest);

            return (paymentResult.SessionId, paymentResult.CheckoutUrl);



        }
        public async Task<(List<(int productId, int quantity)> products, decimal totalPriceOfProducts)>
 CheckExistenceOfOrderItem(List<OrderItemRequestDto> cartItemRequests, int shopId)
        {
            var errors = new List<(string name, string reason)>();
            var products = new List<(int productId, int quantity)>();
            decimal totalPriceOfProducts = 0;

            foreach (var item in cartItemRequests)
            {
                var productDB = await unityOfWork.ProductRepository.GetProductById(item.ProductId);

                // product not found
                if (productDB == null)
                {
                    errors.Add(($"ProductId {item.ProductId}", "Product not found"));
                    continue;
                }

                // wrong shop
                if (productDB.shop_id != shopId)
                {
                    errors.Add((productDB.name, "Product does not belong to this shop"));
                    continue;
                }

                // quantity not enough
                if (productDB.quentity < item.Quantity)
                {
                    errors.Add((productDB.name, $"Available only {productDB.quentity}"));
                    continue;
                }

                // valid
                products.Add((item.ProductId, item.Quantity));
                totalPriceOfProducts += productDB.price * item.Quantity;
            }

            // after finishing loop
            if (errors.Any())
            {
                throw new Exception(
                    "Some products are not valid: " +
                    string.Join(", ", errors.Select(e => $"{e.name}: {e.reason}")));
            }

            return (products, totalPriceOfProducts);
        }

        public async Task<decimal> calculateDeliveryPrice(int deliveryId, int shopId, decimal orderLatitude, decimal orderLongitude)
        {
            var location = await unityOfWork.ShopRepository.GetShopLocationById(shopId);
            if (location == null)
            {
                throw new Exception("shop location not found");
            }
            var (shopLatitude, shopLongitude) = location.Value;
            var delivery = await unityOfWork.DelivaryRepository.GetDeliveryByDeliveryId(deliveryId);
            if (delivery == null)
            {
                throw new Exception("delivery not found");
            }
            decimal pricePerKm = delivery.price_delivery_per_km;
            var routeInfo = await googleMapService.GetRouteInfoAsync(shopLatitude, shopLongitude, orderLatitude, orderLongitude);
            if (routeInfo == null)
            {

                throw new  DomainException("the route not found or the map api failled ");
            }
            decimal distanceInKm = (decimal)routeInfo.Distance / 1000; // Convert distance from meters to kilometers
            decimal deliveryCost = distanceInKm * pricePerKm;
            return deliveryCost;

        }

        // This method will be called by the payment gateway webhook to update the order status based on the payment result

        public async Task HandlePaymentWebhookAsync(GatewayPayment gatwayPayment)
        {
            var order = await unityOfWork.paymentAndOrderRepository.GetOrderByOrderId(gatwayPayment.OrderId);
            if (order == null)
            {
                throw new Exception("order not found");
            }
            //if (gatwayPayment.Status.ToLower() == "failed")
            //{
            //    throw new Exception("sory your no receive money ");
            //}
            //if (order.orderStatus == "paid")
            //{
            //    throw new Exception("you paid befor");
            //}
            await unityOfWork.paymentAndOrderRepository.updateStatusOnClientOrder(order.orderId, "paid");
            var PayInD = new Domain.Entities.Payment.PayIn
            {

                OrderId = order.orderId,
                Amount = gatwayPayment.Amount,
                PaymentDate = DateTime.UtcNow,
                PaymentMethod = "FakeGateway",
                Status = gatwayPayment.Status,
            };
            await unityOfWork.paymentRepository.RegisterPayIn(PayInD);

            var shopInfo = await unityOfWork.ShopRepository.GetShopByShopOwnerIdOrShopId(shopId:order.shopId);
            if (shopInfo == null)
            {
                throw new Exception("shop not found");
            }
            var shopownerPersonId = await unityOfWork.PersonRepository.GetPersonIdByShopOwnerId(shopInfo.shopownerid);
            if (shopownerPersonId == null)
            {
                throw new Exception("shop owner not found");
            }

            var shopOwnerInfo = await unityOfWork.PersonRepository.GetPersonById(shopownerPersonId.Value);
            if (shopOwnerInfo == null)
            {
                throw new Exception("shop owner not found");
            }
            if (shopOwnerInfo.Email != null)
            {
                await emailservice.SendEmailAsync(shopOwnerInfo.Email, "New Order Paid", $"A new order with ID {order.orderId} has been paid. Please prepare the order for delivery.");
            }
            else
            {
                await twilioMessageService.SendSmsAsync($"A new order with ID {order.orderId} has been paid. Please prepare the order for delivery.", shopOwnerInfo.PhoneNumber);
            }

            var deliveryProviderInfo = await unityOfWork.DelivaryRepository.GetDeliveryByDeliveryId(order.deliveryProviderId);
            if (deliveryProviderInfo == null)
            {
                throw new Exception("delivery provider not found");
            }
            var deliveryPersonInfo = await unityOfWork.PersonRepository.GetPersonById(deliveryProviderInfo.person_id);
            if (deliveryPersonInfo == null)
            {
                throw new Exception("delivery person not found");
            }
            if (deliveryPersonInfo.Email != null)
            {
                await emailservice.SendEmailAsync(deliveryPersonInfo.Email, "New Delivery Task", $"You have a new delivery task for order ID {order.orderId}. Please check the app for details.");
            }
            else
            {
                await twilioMessageService.SendSmsAsync($"You have a new delivery task for order ID {order.orderId}. Please check the app for details.", deliveryPersonInfo.PhoneNumber);

            }

        }

        public async Task<List<returnOrderForShopDto>> ReturnOrdersForShop()
        {
            int userId = usercontext.GetUserId();
            string role = usercontext.GetUserRole();
            if (role != "shopowner")
            {
                throw new Exception("user is not shop owner");

            }
            var shopownerId = await unityOfWork.PersonRepository.GetShopOwnerIdByPersonId(userId);
            if (shopownerId == null)
            {
                throw new Exception("shop owner not found");
            }
            var shopId = await unityOfWork.ShopRepository.GetShopIDByShopownerId(shopownerId.Value);
            if (shopId == null)
            {
                throw new Exception("shop not found");
            }
            var ListOfOrders = await this.ReturnOrderByShopId(shopId.Value);
            return ListOfOrders;
        }
        public async Task<List<returnOrderForShopDto>> ReturnOrderByShopId(int shopId)
        {

            var orders = await unityOfWork.paymentAndOrderRepository.GetOrdersRequiredForsopOrDelivery(shopId);
            var returnOrdersForShop = new List<returnOrderForShopDto>();
            foreach (var order in orders)
            {
                var OrderForShopDto = new returnOrderForShopDto();

                var deliveryInfo = await unityOfWork.DelivaryRepository.GetDeliveryByDeliveryId(order.deliveryProviderId);
                if (deliveryInfo == null)
                {
                    throw new Exception("delivery provider not found");
                }
                var deliveryPersonInfo = await unityOfWork.PersonRepository.GetPersonById(deliveryInfo.person_id);
                OrderForShopDto.deliveryname = deliveryPersonInfo.FirstName + " " + deliveryPersonInfo.LastName;
                OrderForShopDto.deliveryemail = deliveryPersonInfo.Email;
                OrderForShopDto.deliveryphone = deliveryPersonInfo.PhoneNumber;
                OrderForShopDto.OrderId = order.orderId;
                OrderForShopDto.TotalPrice = order.ProductTotalCost;
                returnOrdersForShop.Add(OrderForShopDto);

            }
            return returnOrdersForShop;


        }
        public async Task<List<returnOrderForDeliveryDto>> ReturnOrdersForDelivery()
        {
            int userid = usercontext.GetUserId();
            string role = usercontext.GetUserRole();
            if (role != "delivery")
            {
                throw new Exception("user is not delivery");
            }
            var deliveryId = await unityOfWork.PersonRepository.GetDeliveryIdByPersonId(userid);
            if (deliveryId == null)
            {
                throw new Exception("delivery not found");
            }
            var ListOfOrders = await this.ReturnOrderForDelivery(deliveryId.Value);
            return ListOfOrders;
        }

        public async Task<List<returnOrderForDeliveryDto>> ReturnOrderForDelivery(int deliveryId)
        {

            var orders = await unityOfWork.paymentAndOrderRepository.GetOrdersRequiredForsopOrDelivery(0, deliveryId);
            var returnOrdersForDelivery = new List<returnOrderForDeliveryDto>();
            foreach (var order in orders)
            {
                var OrderForDeliveryDto = new returnOrderForDeliveryDto();
                var shop = await unityOfWork.ShopRepository.GetShopByShopOwnerIdOrShopId(shopId: order.shopId);
                if (shop == null)
                {
                    throw new Exception("shop not found");
                }
                var personShopOwnerId = await unityOfWork.PersonRepository.GetPersonIdByShopOwnerId(shop.shopownerid);
                if (personShopOwnerId == null)
                {
                    throw new Exception("shop owner not found");
                }
                var personShopOwnerInfo = await unityOfWork.PersonRepository.GetPersonById(personShopOwnerId.Value);

                OrderForDeliveryDto.shopname = personShopOwnerInfo.FirstName + " " + personShopOwnerInfo.LastName;
                OrderForDeliveryDto.shoplatitude = shop.shoplatitude;
                OrderForDeliveryDto.shoplongitude = shop.shopLongitude;
                OrderForDeliveryDto.HashDeliveryShopPin = order.shopdeliverypin;
                OrderForDeliveryDto.deliveryCost = order.DeliveryCost;

                OrderForDeliveryDto.clientlatitude = order.latitude;
                OrderForDeliveryDto.clientlongitude = order.longitude;

                var clientPersonId = await unityOfWork.PersonRepository.GetPersonIdByClientId(order.clientId);
                if (clientPersonId == null)
                {
                    throw new Exception("client not found");
                }
                var clientPerson = await unityOfWork.PersonRepository.GetPersonById(clientPersonId.Value);
                OrderForDeliveryDto.clientname = clientPerson.FirstName + " " + clientPerson.LastName;
                OrderForDeliveryDto.clientphonenumber = clientPerson.PhoneNumber;
                OrderForDeliveryDto.clientemail = clientPerson.Email;
                returnOrdersForDelivery.Add(OrderForDeliveryDto);
            }
            return returnOrdersForDelivery;
        }


        public async Task<List<returnItemOrder>> GetItemsOfOrder(int orderId)
        {
            int userId = usercontext.GetUserId();
            string role = usercontext.GetUserRole();
            if (role != "shopowner" && role != "delivery" && role != "admin")
            {
                throw new Exception("user is not authorized to view order items");
            }
            var orderItems = await unityOfWork.paymentAndOrderRepository.GetItemsOfOrder(orderId);
            var returnItems = new List<returnItemOrder>();
            foreach (var item in orderItems)
            {
                var product = await unityOfWork.ProductRepository.GetProductById(item.productId);
                var returnItem = new returnItemOrder
                {
                    orderDetailId = item.Id,
                    productId = item.productId,
                    productName = product.name,
                    quantity = item.quantity,
                    price = product.price
                };
                returnItems.Add(returnItem);
            }
            return returnItems;

        }
        public async Task takeorderfromshoptodelivery(RecievefromShopToDeliveryDto recievefromShopToDeliverydto)
        {
            int _userId;
            string _role;

            _userId = usercontext.GetUserId();
            _role = usercontext.GetUserRole();
            if (_role != "shopowner")
            {
                throw new Exception("user is not shop owner");
            }


            //check if userid is shopowner
            var shopownerId = await unityOfWork.PersonRepository.GetShopOwnerIdByPersonId(_userId);
            if (shopownerId == null)
            {
                throw new Exception("shop owner not found");
            }
            var shop = await unityOfWork.ShopRepository.GetShopByShopOwnerIdOrShopId(shopownerId.Value);
            if (shop == null)
            {
                throw new Exception("shop not found");
            }
            await unityOfWork.paymentAndOrderRepository.deliveryreciveorder(recievefromShopToDeliverydto, shop.shopid);





        }

        public async Task RecieveOrderFromDeliveryTOClient(RecieveFromDeliveryToClientDto recievefromDeliveryToClient)
        {
            int userId;
            string role;

            userId = usercontext.GetUserId();
            role = usercontext.GetUserRole();
            if (role != "delivery")
            {
                throw new Exception("user is not delivery");
            }
            await unityOfWork.paymentAndOrderRepository.RecieveOrederFromDeliveryToClient(recievefromDeliveryToClient);
        }
    }
}
                        



                
            
    




        // +++++++++++++++++++++++++++++++++++++++++new end +++++++++++++++++++++++++++++++++++++++++++++
        



