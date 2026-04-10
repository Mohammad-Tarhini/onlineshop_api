using onlineshopowner_api.Application.Dtos;
using onlineshopowner_api.Application.Dtos.Payment;
using onlineshopowner_api.Application.Dtos.PaymentAndOrder;
using onlineshopowner_api.Application.Interfaces.Iservices;
using onlineshopowner_api.Domain.Interfaces.IExternalServices;
using onlineshopowner_api.Infrastructure.ExternalServices.Payment;
using onlineshopowner_api.Infrastructure.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.SessionState;

namespace onlineshopowner_api.Controllers
{
    public class OrderAndPaymentController : ApiController
    {
        private readonly IOrderServices _orderServices;
        private readonly IFakeGatewayService _fakeGatewayService;
        public OrderAndPaymentController(IOrderServices orderServices, IFakeGatewayService fakeGatewayService)
        {
            _orderServices = orderServices;
            _fakeGatewayService = fakeGatewayService;
        }
        //+++++++++++++++++++++++++++++++++++++++++++new++++++++++++++++++++++
        [JwtAuthorize(Roles = "client")]
        [HttpPost]
        [Route("api/order/registerOrder")]
        public async Task<IHttpActionResult> RegisterOrder([FromBody] PayDto dto)
        {
            var (sessionId, checkOutUrl) = await _orderServices.registerOrder(dto);
            return Ok(new { sessionId, checkOutUrl });
        }
        //fake payment endpoint for testing
        [JwtAuthorize(Roles = "client")]
        [HttpGet]
        [Route("api/gateway/paymentProcess")]
        public async Task<IHttpActionResult> ProcessPayment([FromUri] string sessionId, [FromUri] string cardNumber=null)
        {
          var gateWayPayment=  await _fakeGatewayService.ProcessPaymentAsync(sessionId, cardNumber);
          await _orderServices.HandlePaymentWebhookAsync(gateWayPayment);
          return Ok("Payment processed");
        }
        //[AllowAnonymous]
        //[HttpPost]
        //[Route("api/gateway/webhook")]
        //public async Task<IHttpActionResult> PaymentWebhook([FromBody] GatewayPayment gatewayPayment)
        //{

        //    await _orderServices.HandlerPaymentWebhookAsync(gatewayPayment);
        //    return Ok("Webhook received and processed");
        //}
        [JwtAuthorize(Roles = "shopowner,admin")]
        [HttpGet]
        [Route("api/order/getNewordersForShop")]
        public async Task<IHttpActionResult> GetNewOrdersForShop()
        {
            var ordersForShop = await _orderServices.ReturnOrdersForShop();
            if (ordersForShop == null || !ordersForShop.Any())
            {
                return Ok("No new orders for the shop.");
            }
            return Ok(ordersForShop);

        }
        [JwtAuthorize(Roles = "delivery,admin")]
        [HttpGet]
        [Route("api/order/getOrdersForDelivery")]
        public async Task<IHttpActionResult> GetOrdersForDelivery()
        {
            var ordersForDelivery = await _orderServices.ReturnOrdersForDelivery();
            if (ordersForDelivery == null || !ordersForDelivery.Any())
            {
                return Ok("No new orders for delivery.");
            }
            return Ok(ordersForDelivery);
        }
        [HttpPost]
        [Route("api/order/getItemsOfOrder")]
        public async Task<IHttpActionResult> GetItemsOfOrder([FromUri] int orderId)
        {
            var itemsOfOrder = await _orderServices.GetItemsOfOrder(orderId);
            if (itemsOfOrder == null || !itemsOfOrder.Any())
            {
                return Ok("No items found for this order.");
            }
            return Ok(itemsOfOrder);

        }

        [JwtAuthorize(Roles = "Shopowner")]
        [HttpPost]
        [Route("api/order/giveOrderFromShopToDelivery")]
        public async Task<IHttpActionResult> GiveOrderFromShopToDelivery([FromBody] RecievefromShopToDeliveryDto recievefromShopToDeliverydto)
        {
            await _orderServices.takeorderfromshoptodelivery(recievefromShopToDeliverydto);
            return Ok("Order assigned to delivery successfully.");
        }
        [JwtAuthorize(Roles = "client")]
        [HttpPost]
        [Route("api/order/giveOrderFromDeliveryToClient")]
        public async Task<IHttpActionResult> GiveOrderFromDeliveryToClient([FromBody] RecieveFromDeliveryToClientDto recievefromDeliveryToClient)
        {
            await _orderServices.RecieveOrderFromDeliveryTOClient(recievefromDeliveryToClient);
            return Ok("Order marked as delivered to client successfully.");
        }
    }
}








