using Antlr.Runtime.Tree;
using Newtonsoft.Json;
using onlineshopowner_api.Application.Dtos;
using onlineshopowner_api.Application.Dtos.Payment;
using onlineshopowner_api.Application.Dtos.PaymentAndOrder;
using onlineshopowner_api.Application.Interfaces.Iservices;
using onlineshopowner_api.Application.Validatorandclean;
using onlineshopowner_api.Domain.Constant;
using onlineshopowner_api.Domain.Entities.PaymentAndOrder;
using onlineshopowner_api.Domain.Interfaces.IExternalServices;
using onlineshopowner_api.Domain.Interfaces.IRepository;
using onlineshopowner_api.Infrastructure.ExternalServices;
using onlineshopowner_api.Infrastructure.Repositories;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity.ModelConfiguration.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using Twilio.Rest.Trunking.V1;

namespace onlineshopowner_api.Application.Services
{
    public class PayAndRegisterOrder : IOrderServices
    {

        private readonly IUserContextService _usercontext;
        private readonly IUnityOfWork unityOfWork;
        private readonly IGoogleMapService googleMapService;
        private readonly ITwilioMessageService twilioMessageService;

        public PayAndRegisterOrder(IUnityOfWork unityOfWork, IUserContextService userContext, IGoogleMapService googleMapService, ITwilioMessageService twilioMessageService)
        {
            this.unityOfWork = unityOfWork;
            this.googleMapService = googleMapService;
            _usercontext = userContext;
            this.twilioMessageService = twilioMessageService;
        }

        public async Task<(bool issucess, List<CartItemCheckResponseDto> cartItemsResponse, decimal Totalprice, string message)> CheckCartItemAvailability(CartAvailabilityRequestDto dto)
        {
            int shopid = dto.ShopId;
            var cartItemRequestDtos = dto.CartItemDtos;
            int _userId;
            string _role;
            int shopownerid;
            decimal totalprice = 0;
            try
            {
                _userId = _usercontext.GetUserId();
                _role = _usercontext.GetUserRole();
            }
            catch (UnauthorizedAccessException ex)
            {
                return (false, null, 0, ex.Message);
            }
            try
            {
                _role = _role.ToLower();


                if (_role != "client") return (false, null, 0, "user is not client  ");

                var result = new List<CartItemCheckResponseDto>();
                foreach (var item in cartItemRequestDtos)
                {
                    var resultproductDB = await unityOfWork.ProductRepository.GetProductById(item.ProductId);
                    if (resultproductDB == null)
                    {
                        result.Add(new CartItemCheckResponseDto
                        {
                            ProductId = item.ProductId,
                            ProductName = item.ProductName,
                            IsAvailable = false,
                            Reason = "Product is not found "
                        });
                        continue;
                    }
                    if (resultproductDB.Value.shop_id != shopid)
                    {
                        result.Add(new CartItemCheckResponseDto
                        {
                            ProductId = item.ProductId,
                            ProductName = item.ProductName,
                            IsAvailable = false,
                            Reason = "the product not in shhopid"
                        });
                        continue;
                    }
                    if (!resultproductDB.IsSuccess)
                    {
                        result.Add(new CartItemCheckResponseDto
                        {
                            ProductId = item.ProductId,
                            ProductName = item.ProductName,
                            IsAvailable = false,

                            Reason = "error in database "

                        });
                        continue;
                    }
                    if (!resultproductDB.IsFound)
                    {
                        result.Add(new CartItemCheckResponseDto
                        {
                            ProductId = item.ProductId,
                            ProductName = item.ProductName,
                            IsAvailable = false,
                            Reason = "product not found"
                        });
                        continue;
                    }
                    if (!string.Equals(resultproductDB.Value.status?.Trim(), "available", StringComparison.OrdinalIgnoreCase))
                    {
                        result.Add(new CartItemCheckResponseDto
                        {
                            ProductId = item.ProductId,
                            ProductName = item.ProductName,
                            IsAvailable = false,
                            CurrentPrice = resultproductDB.Value.price,
                            Reason = "is not available"
                        });
                        continue;
                    }
                    int dif = resultproductDB.Value.quentity - item.Quantity;
                    if (dif < 0)
                    {
                        result.Add(new CartItemCheckResponseDto
                        {
                            ProductId = item.ProductId,
                            ProductName = item.ProductName,
                            IsAvailable = false,
                            Reason = "the available only " + dif
                        });
                        continue;
                    }
                    result.Add(new CartItemCheckResponseDto
                    {
                        ProductId = item.ProductId,
                        ProductName = resultproductDB.Value.name,
                        IsAvailable = true,
                        AvailableQuantity = resultproductDB.Value.quentity,
                        RequiredQuantity = item.Quantity,
                        CurrentPrice = resultproductDB.Value.price,

                    });
                    totalprice += resultproductDB.Value.price;



                }
                return (true, result, totalprice, "sucess");

            }
            catch (Exception ex)
            {
                return (false, null, 0, ex.Message);
            }

        }

        public async Task<(bool issuccess, string message)> PaymentAndRegisterOrder(PayDto paydto)
        {
            try
            {
                if (paydto.OrderLocation == null)
                    return (false, "OrderLocation is null");

                if (paydto.OrderLocation.shopid == 0)
                    return (false, "shopid is missing in OrderLocation");
                if (paydto.Items == null || !paydto.Items.Any())
                    return (false, "Cart items are missing");

                var cartavailblerequest = new CartAvailabilityRequestDto
                {
                    CartItemDtos = paydto.Items,
                    ShopId = paydto.OrderLocation.shopid,
                };
                var (issucess, itemslist, totalpriceofitem, message) = await CheckCartItemAvailability(cartavailblerequest);
                if (!issucess) { return (false, message+"ooo"); }

                var resultcheckdbShopLocation = await unityOfWork.ShopRepository.GetShopLocationById(paydto.OrderLocation.shopid);
                if (resultcheckdbShopLocation == null) return (false, "errrrrrrrr");
                if (!resultcheckdbShopLocation.IsSuccess) return (false, resultcheckdbShopLocation.Error+"k1");
                if (!resultcheckdbShopLocation.IsFound) return (false, "shop is not found");
                var (shoplatitude, shoplangitude) = resultcheckdbShopLocation.Value;
                var routeinfodto = await googleMapService.GetRouteInfoAsync(shoplatitude, shoplangitude, paydto.OrderLocation.latitude, paydto.OrderLocation.longitude);
               // if (routeinfodto == null) return (false, "the route is not found");

                decimal rateperkm = 0.5m;
                //  decimal deliverycost = (decimal)routeinfodto.Distance * rateperkm;
                decimal deliverycost = 0;
                //   decimal TotalCost = deliverycost + totalpriceofitem;
                decimal TotalCost = totalpriceofitem;
                //her i have to write the code all call for helper the api for payment 

                DataTable itemcarttable = new DataTable();
                itemcarttable.Columns.Add("productid", typeof(int));
                itemcarttable.Columns.Add("quantity", typeof(int));
                foreach (var item in paydto.Items)
                {
                    var itemrespond = itemslist.FirstOrDefault(p => p.ProductId == item.ProductId);
                    if (itemrespond == null) continue;
                    if (itemrespond.IsAvailable == false) continue;

                    itemcarttable.Rows.Add(item.ProductId, item.Quantity);
                }

                int userId = _usercontext.GetUserId();
                Random deliveyrnd = new Random();
                var deliveryRandom = deliveyrnd.Next(1000, 9999).ToString();

                Random clientrnd = new Random();
                var clientRandom = clientrnd.Next(1000, 9999).ToString();


                var (Registersuccess, errormassegeforregisterorder) = await unityOfWork.paymentAndOrderRepository.RegisterOrder(userId, totalpriceofitem, deliverycost, paydto.OrderLocation.latitude, paydto.OrderLocation.longitude, paydto.OrderLocation.shopid, paydto.DeliveryProviderId, itemcarttable, deliveryRandom, clientRandom,paydto.paymentmethode);
                if (!Registersuccess) return (false, errormassegeforregisterorder+"k2");
              
                var ResultPerson = await unityOfWork.PersonRepository.GetPersonByPersonId(userId);
                if (ResultPerson == null) return (false, errormassegeforregisterorder+"k3");
                if (ResultPerson.IsSuccess == false) return (false, "there error appear in get person by person id");
                if (ResultPerson.IsFound == false) return (false, "theperson is not foumd immposible");
                var clientname = ResultPerson.Value.FirstName + " " + ResultPerson.Value.LastName;
                var clientphonenumber = ResultPerson.Value.PhoneNumber;
                var resultshopownerphonenumberandemailandShopname = await unityOfWork.ShopRepository.GetPhoneNumberAndEmailbyShopid(paydto.OrderLocation.shopid);
                var shopname = resultshopownerphonenumberandemailandShopname.Value.shopname;
                var shopphonenumber = resultshopownerphonenumberandemailandShopname.Value.phonenumber;
                var shopemail = resultshopownerphonenumberandemailandShopname.Value.email;
                var resultNumberEmaiTypeDelivery = await unityOfWork.DelivaryRepository.GetPhoneAndEmailForDelivery(paydto.DeliveryProviderId);
                if (!resultNumberEmaiTypeDelivery.IsSuccess)
                {
                    //iasuhfuiahu
                }
                if (!resultNumberEmaiTypeDelivery.IsFound)
                {
                    //adpsjopjfioe;
                }
                if (resultNumberEmaiTypeDelivery.Value.phonenumber == null)
                {
                    //asdiohguihesuig
                }
                var buildmessagefordelivery = new StringBuilder();
                if (resultNumberEmaiTypeDelivery.Value.deliverytype == "person" || resultNumberEmaiTypeDelivery.Value.deliverytype == "agent")
                {

                    buildmessagefordelivery.Append("please  if you can take this order ");
                    buildmessagefordelivery.AppendLine("shoplocation:   ");
                    buildmessagefordelivery.Append($"https://www.google.com/maps?q={shoplatitude},{shoplangitude}");
                    buildmessagefordelivery.AppendLine($"shopname:{shopname},phonenumber:{shopphonenumber},shopemail:{shopemail}");
                    buildmessagefordelivery.AppendLine("clientLocation:  ");
                    buildmessagefordelivery.AppendLine($"https://www.google.com/maps?q={paydto.OrderLocation.latitude},{paydto.OrderLocation.longitude}");
                    buildmessagefordelivery.AppendLine($"clientname:{clientname},clientphonenumber:{clientphonenumber}");
                    buildmessagefordelivery.AppendLine();

                }

                var buildmessageforshop = new StringBuilder();
                foreach (var item in paydto.Items)
                {
                    var theavailableitem = itemslist.FirstOrDefault(i => i.ProductId == item.ProductId);
                    if (theavailableitem == null)
                        continue;
                    if (theavailableitem.IsAvailable == false)
                        continue;
                    buildmessageforshop.Append(item.ProductName);
                    buildmessagefordelivery.Append(item.ProductName);
                    buildmessageforshop.Append(":");
                    buildmessagefordelivery.Append(":");
                    buildmessageforshop.Append(item.Quantity);
                    buildmessagefordelivery.Append(item.Quantity);
                    buildmessagefordelivery.AppendLine();
                    buildmessageforshop.AppendLine();

                }
                buildmessageforshop.AppendLine(totalpriceofitem.ToString());


                if (resultNumberEmaiTypeDelivery.Value.deliverytype == "shop")
                {
                    buildmessageforshop.AppendLine("please take them to this location");
                    buildmessageforshop.AppendLine($"https://www.google.com/maps?q={paydto.OrderLocation.latitude},{paydto.OrderLocation.longitude}");
                }
                else
                {
                    buildmessageforshop.AppendLine("the delivery is ");
                    buildmessageforshop.AppendLine($"deliveryname:{resultNumberEmaiTypeDelivery.Value.name}");
                    buildmessageforshop.Append($"phonenumber: {resultNumberEmaiTypeDelivery.Value.phonenumber}");
                    buildmessageforshop.Append($"email:{resultNumberEmaiTypeDelivery.Value.email}");
                }
                buildmessagefordelivery.AppendLine($"the password you should give for shop to take the order:{deliveryRandom}");
                string messageforshop = buildmessageforshop.ToString();
                string messagefordelivery = buildmessagefordelivery.ToString();

                //have here many problem 
                var resultmessageforshop = await twilioMessageService.SendSmsAsync(messageforshop, shopphonenumber);
                if (resultmessageforshop == false)
                {
                    //i have to put a scinario
                }
                var resultmessageofrdelivery = await twilioMessageService.SendSmsAsync(resultNumberEmaiTypeDelivery.Value.phonenumber, messagefordelivery);
                if (resultmessageofrdelivery == false)
                {
                    //asfhuihfuaf
                }


                return (true, clientRandom);


            }
            catch (Exception ex)
            {
                return (false, ex.Message+"kkkkkk");
            }


        }

        public async Task<(bool issuccess, bool isempty, List<Order>, string message)> GetOrdersForShop()
        {
            int _userId;
            string _role;
            try
            {
                _userId = _usercontext.GetUserId();
                _role = _usercontext.GetUserRole();
                if (_role != "shopowner") return (false, false, null, "the role is wrong ");
            }
            catch (UnauthorizedAccessException ex)
            {
                return (false, false, null, ex.Message+"b8");
            }
            //check if userid is shopowner
            Domain.Entities.ShopOwner shopowner;
            try
            {
                var personResultCheckdb = await unityOfWork.PersonRepository.GetPersonByPersonId(_userId);

                if (!personResultCheckdb.IsSuccess) return (false, false, null, personResultCheckdb.Error+"b9");
                if (!personResultCheckdb.IsFound) return (false, false, null, personResultCheckdb.Error+"b10");
                try
                {
                    var shopownerResultCheckdb = await unityOfWork.PersonRepository.GetShopOwnerByPersonAsync(personResultCheckdb.Value);
                    if (!shopownerResultCheckdb.IsSuccess) return (false, false, null, shopownerResultCheckdb.Error+"b1");
                    if (!shopownerResultCheckdb.IsFound) return (false, false, null, shopownerResultCheckdb.Error+"b2");
                    shopowner = shopownerResultCheckdb.Value;

                }
                catch (Exception ex) { return (false, false, null, ex.Message+"b7"); }



                var shop = await unityOfWork.ShopRepository.GetShopByShopOwner(shopowner);
                if (!shop.IsSuccess) return (false, false, null, shop.Error+"b3");
                if (!shop.IsFound) return (false, false, null, shop.Error+"b4");
                int shopid = shop.Value.shopid;
                var resultNewOrders = await unityOfWork.paymentAndOrderRepository.GetNewOrder(shopid);
                if (resultNewOrders == null) return (false, false, null, "resultNewOrders.Error");
                if (!resultNewOrders.IsSuccess) return (false, false, null, resultNewOrders.Error+"b20");
                if (!resultNewOrders.IsFound) return (true, false, null, "no order");
                return (true, true, resultNewOrders.Value, "mabrouk");
            }
            catch (Exception ex) { return (false, false, null, ex.Message+"b6"); }
        }
        public async Task<(bool issuccess, bool isfound, List<OrderForDelivery>, string message)> GetOrdersOfDelivery()
        {
            try
            {
                int user = _usercontext.GetUserId();
                string Roler = _usercontext.GetUserRole();

                if (Roler != "delivery")
                    return (false, false, null, "you are not  not delivery usre");
                var resultgetorderfordelivery = await unityOfWork.paymentAndOrderRepository.GetOrdersForDelivery(user);
                if (resultgetorderfordelivery == null) return (false, false, null, "no thing return from repo");
                if (!resultgetorderfordelivery.IsSuccess) return (false, false, null, resultgetorderfordelivery.Error+"not ss");
                if (!resultgetorderfordelivery.IsFound)
                    return (true, false, null, "not found ");
                //foreach(var order in resultgetorderfordelivery.Value)
                //{
                //    var orderfordeliverydto = new OrderForDeliveryDto
                //    {
                //        clientemail = order.clientemail,
                //        clientlatitude = order.clientlatitude,
                //        clientlongitude = order.clientlongitude,
                //        clientname = order.clientname,
                //        clientphonenumber = order.clientphonenumber,
                //        HashDeliveryShopPin = order.HashDeliveryShopPin,
                //        orderdate = order.orderdate,
                //        orderid = order.orderid,
                //        ShopEmail = order.ShopEmail ,
                //        shopid = order.shopid,
                //        shoplatitude = order.shoplatitude,
                //        shoplongitude = order.shoplongitude,
                //        shopname = order.shopname,
                //        ShopPhoneNumber=order.ShopPhoneNumber

                //    };
                //}
                return (true, true, resultgetorderfordelivery.Value, "mabrouk");


            }
            catch (Exception ex)
            {
                return (false, false, null, ex.Message+"ew");
            }
        }


        public async Task<(bool issuccess, bool isfound, List<CartItem>, string message)> GetItemsfororder(int orderid)
        {
            int _userId;
            string _role;
            try
            {
                _userId = _usercontext.GetUserId();
                _role = _usercontext.GetUserRole();
                if (_role != "shopowner") return (false, false, null, "the role is wrong ");
            }
            catch (UnauthorizedAccessException ex)
            {
                return (false, false, null, ex.Message);
            }
            //check if userid is shopowner
            Domain.Entities.ShopOwner shopowner;
            try
            {
                var personResultCheckdb = await unityOfWork.PersonRepository.GetPersonByPersonId(_userId);

                if (!personResultCheckdb.IsSuccess) return (false, false, null, personResultCheckdb.Error);
                if (!personResultCheckdb.IsFound) return (false, false, null, personResultCheckdb.Error);
                try
                {
                    var shopownerResultCheckdb = await unityOfWork.PersonRepository.GetShopOwnerByPersonAsync(personResultCheckdb.Value);
                    if (!shopownerResultCheckdb.IsSuccess) return (false, false, null, shopownerResultCheckdb.Error);
                    if (!shopownerResultCheckdb.IsFound) return (false, false, null, shopownerResultCheckdb.Error);
                    shopowner = shopownerResultCheckdb.Value;

                }
                catch (Exception ex) { return (false, false, null, ex.Message); }

                var resultItemsoforder = await unityOfWork.paymentAndOrderRepository.GetItemsOfOrder(orderid);
                if (resultItemsoforder == null) return (false, false, null, "is emptu mind");
                if (!resultItemsoforder.IsSuccess) return (false, false, null, resultItemsoforder.Error);
                if (!resultItemsoforder.IsFound) { return (true, false, null, ""); }
                return (true, true, resultItemsoforder.Value, "mabrouk");


            }
            catch (Exception ex)
            {
                return (false, false, null, ex.Message);
            }

        }
        public async Task<(bool issucces, string message)> takeorderfromshoptodelivery(RecievefromShopToDelivery recievefromShopToDeliverydto)
        {
            int _userId;
            string _role;
            try
            {
                _userId = _usercontext.GetUserId();
                _role = _usercontext.GetUserRole();
                if (_role != "shopowner") return (false, "the role is wrong ");
            }
            catch (UnauthorizedAccessException ex)
            {
                return (false, ex.Message);
            }
            //check if userid is shopowner
            Domain.Entities.ShopOwner shopowner;
            try
            {
                var personResultCheckdb = await unityOfWork.PersonRepository.GetPersonByPersonId(_userId);

                if (!personResultCheckdb.IsSuccess) return (false, personResultCheckdb.Error);
                if (!personResultCheckdb.IsFound) return (false, personResultCheckdb.Error);
                try
                {
                    var shopownerResultCheckdb = await unityOfWork.PersonRepository.GetShopOwnerByPersonAsync(personResultCheckdb.Value);
                    if (!shopownerResultCheckdb.IsSuccess) return (false, shopownerResultCheckdb.Error);
                    if (!shopownerResultCheckdb.IsFound) return (false, shopownerResultCheckdb.Error);
                    shopowner = shopownerResultCheckdb.Value;

                }
                catch (Exception ex) { return (false, ex.Message); }
                var shop = await unityOfWork.ShopRepository.GetShopByShopOwner(shopowner);
                if (!shop.IsSuccess) return (false, shop.Error);
                if (!shop.IsFound) return (false, shop.Error);
                int shopid = shop.Value.shopid;

                var (issucess, isStartDelivery, message) = await unityOfWork.paymentAndOrderRepository.deliveryreciveorder(recievefromShopToDeliverydto,shopid);
                if(!issucess)return (false, message);
                if(!isStartDelivery)return(false, message);
                if (isStartDelivery)
                { 
                    //var isSendmessage=await twilioMessageService.SendSmsAsync()
                    return (true, "go man");
                } 
                return(false,null);
            }
            catch (Exception ex)
            {
                return (false, ex.Message);
            }
        }

        public async Task<(bool issuccess,string message)> RecieveOrderFromDeliveryTOClient(RecieveFromDeliveryToClient recievefromDeliveryToClient)
        {
            int _userId;
            string _role;
            try
            {
                _userId = _usercontext.GetUserId();
                _role = _usercontext.GetUserRole();
                if (_role != "delivery") return (false, "the role is wrong ");
                var(issucessrecive,isexuted,message)=await unityOfWork.paymentAndOrderRepository.RecieveOrederFromDeliveryToClient(recievefromDeliveryToClient);
                if(!issucessrecive) return (false, message);
                if (!isexuted) { return (false, message); }
                if (issucessrecive && isexuted) return (true, message);
                return (false, null);
            }
            catch (UnauthorizedAccessException ex)
            {
                return (false, ex.Message);
            }



        }
}   } 

 