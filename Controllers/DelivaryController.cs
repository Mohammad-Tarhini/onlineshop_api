using onlineshopowner_api.Application.Dtos;
using onlineshopowner_api.Application.Dtos.DeliveryDtos;
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
    public class DelivaryController : ApiController
    {
        private IDeliveryServices deliveryServices;

        public DelivaryController(IDeliveryServices deliveryServices)
        {
            this.deliveryServices = deliveryServices;
            
        }

        [HttpPost]
        [Route("api/delivery/adddeliveryagent")]
        public async Task<IHttpActionResult> AddDelivaryAgent([FromBody] DeliveryAgentDto deliveryAgentDto)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);
                var (issuccess, isalreadyexist, message) = await deliveryServices.AddDeliveryAgent(deliveryAgentDto);
                if (!issuccess) return BadRequest(message);
                if (isalreadyexist) return BadRequest(message);
                return Ok();
            }
            catch (Exception ex) 
            {
                return BadRequest(ex.Message);
            }
        }
        
        [HttpPost]
        [Route("api/delivery/adddeliveryperson")]
        public async Task<IHttpActionResult> AddDeliveryPerson([FromBody] DeliveryPersonDto deliveryPersonDto )
        {
            if(!ModelState.IsValid){ return BadRequest(ModelState); }
            var(issuccess,isalreadyexist,message)= await  deliveryServices.AddPersonDelivery(deliveryPersonDto);
            if(!issuccess){return BadRequest(message); }
            if(isalreadyexist){return BadRequest(ModelState); }
            return Ok();
        }
        [JwtAuthorize(Roles = "shopowner")]
        [HttpPost]
        [Route("api/delivery/adddeliveryshop")]

        public async Task<IHttpActionResult> AddDeliveryshop([FromBody] DeliveryShopDto deliveryShopDto)
        {
            if (!ModelState.IsValid) { return BadRequest(ModelState); }
            var (issuccess, isalreadyexist, message) = await deliveryServices.AddShopDelivery(deliveryShopDto);
            if (!issuccess) { return BadRequest(message); }
            if (isalreadyexist) { return BadRequest(ModelState); }
            return Ok();
        }

        [HttpGet]
        [JwtAuthorize(Roles = "client")]
        [Route("api/delivary/")]
        public async Task<IHttpActionResult> GetDelivaryAndPostForlocation([FromBody] OrderLocationDto dto)
        {
            if(!ModelState.IsValid) {return BadRequest(ModelState);}


        }
    }
}
