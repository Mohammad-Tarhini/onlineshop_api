using onlineshopowner_api.Application.Dtos;
using onlineshopowner_api.Application.Dtos.Payment;
using onlineshopowner_api.Application.Dtos.PaymentAndOrder;
using onlineshopowner_api.Application.Interfaces.Iservices;
using onlineshopowner_api.Infrastructure.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;

namespace onlineshopowner_api.Controllers
{
    public class OrderAndPaymentController : ApiController
    {
        private readonly IOrderServices _orderServices;
        public OrderAndPaymentController(IOrderServices orderServices)
        {
            _orderServices = orderServices;
        }
        [JwtAuthorize(Roles = "client")]
        [HttpPost]
        [Route("api/order/checkavailability")]
        public async Task<IHttpActionResult> CheckAvailability([FromBody] CartAvailabilityRequestDto dto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }
                var (issucess, cartitems, totalprice, message) = await _orderServices.CheckCartItemAvailability(dto);
                if (!issucess) { return BadRequest(message); }
                if (issucess)
                {
                    return Ok(new { cartitems, totalprice });

                }
                return BadRequest();
            }
            catch (Exception ex) 
            {
                return BadRequest(ex.Message);
            }

        }
        [JwtAuthorize(Roles = "client")]
        [HttpPost]
        [Route("api/order/pay")]
        public async Task<IHttpActionResult> PaymentAndRegisterOrder([FromBody]PayDto payDto)
        {
            try
            {
                if (!ModelState.IsValid) return BadRequest(ModelState);
                var (issucces, message) = await _orderServices.PaymentAndRegisterOrder(payDto);
                if (!issucces) { return BadRequest(message+"lll"); }
                else if (issucces) return Ok(message+"hi");
                return BadRequest("no no");    
            }catch(Exception ex)
            {
                return BadRequest(ex.Message+"wgy");
            }
                
            
        }

       

        [JwtAuthorize(Roles = "shopowner")]
        [HttpGet]
        [Route("api/order/getordersforshop")]
        public async Task<IHttpActionResult> GetNewOrdersforshop()
        {
            try
            {
                var (issuccess, isfound, orders, message) = await _orderServices.GetOrdersForShop();
                if (!issuccess) { return BadRequest(message+"ss"); }
                if (!isfound) return Ok("no orders");
                if (isfound) return Ok(orders);
                return BadRequest("no no");
            }
            catch (Exception ex) 
            {
                return BadRequest(ex.Message+"dd");
            }
        }
       
        [JwtAuthorize(Roles = "shopowner,delivery")]
        [HttpGet]
        [Route("api/order/getitemsoforder")]
        public async Task<IHttpActionResult> GetItemsOfOrder(int orderid)
        {
            try
            {
                var (issuccess, isfound, items, message) = await _orderServices.GetItemsfororder(orderid);
                if (!issuccess) { return BadRequest(message); }
                if(!isfound) return Ok("no items");
                if(isfound) return Ok(items);
                return BadRequest();
            }
            catch(Exception ex) {  return BadRequest(ex.Message); }

        }
        [JwtAuthorize(Roles ="delivery")]
        [HttpGet]
        [Route("api/order/getordersfordelivery")]
        public async Task<IHttpActionResult> GetOrdersForDelivery()
        {
            try
            {
                var (issuccess,isfound,listorder,message)=await _orderServices.GetOrdersOfDelivery();
                if (!issuccess) { return BadRequest(message+"aa"); }
                if(!isfound) return Ok("no items ");
                if(isfound) return Ok(listorder);
                return BadRequest("dd");

            }catch(Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
     


        [JwtAuthorize(Roles = "shopowner")]
       [HttpPost]
        [Route("api/order/giveorderfromshoptodelivery")]
       public async Task<IHttpActionResult>GiveOrderFromShopToDelivery([FromBody] RecievefromShopToDelivery recievefromShopToDelivery )
        {
            try
            {
                if (!ModelState.IsValid) return BadRequest();
                var (issuccess, message) = await _orderServices.takeorderfromshoptodelivery(recievefromShopToDelivery);
                if (!issuccess) { return BadRequest(message); }
                if (issuccess) return Ok("good");
                return BadRequest();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
       

        [JwtAuthorize(Roles ="delivey")]
        [HttpPost]
        [Route("api/order/giveorderfromdeliverytoclient")]
        public async Task<IHttpActionResult> GiveOrderFromDeliveryToClient(RecieveFromDeliveryToClient recievefromDeliveryToClient)
        {
            try
            {
                if(!ModelState.IsValid) return BadRequest();
                var (issuccess, message) = await _orderServices.RecieveOrderFromDeliveryTOClient(recievefromDeliveryToClient);
                if (!issuccess) { return BadRequest(); }
                if (issuccess) return Ok();
                return BadRequest();

            }catch(Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }







    }
}
