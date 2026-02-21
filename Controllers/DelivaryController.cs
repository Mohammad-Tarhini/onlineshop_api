using onlineshopowner_api.Application.Dtos;
using onlineshopowner_api.Application.Dtos.DeliveryDtos;
using onlineshopowner_api.Application.Interfaces.Iservices;
using onlineshopowner_api.Domain.Entities.Delivery;
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



        [HttpGet]
        [Route("api/delivary/getdeliveryonloction")]
        public async Task<IHttpActionResult> GetDelivaryAcordingForlocation([FromBody] OrderLocationDto dto)
        {
            var deliveries = await deliveryServices.GetDelivaryAcordingForlocationService(dto);
            if(deliveries == null)
            {
                return NotFound();
            }
            return Ok(deliveries);
        }
      
    }
}
