using onlineshopowner_api.Application.Dtos;
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

        

       
        
       


    }
}
