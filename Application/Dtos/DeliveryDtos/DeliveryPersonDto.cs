using onlineshopowner_api.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace onlineshopowner_api.Application.Dtos.DeliveryDtos
{
    public class DeliveryPersonDto
    {
        public PersonDto persondto {  get; set; }
        public DeliveryProviderDto deliveryProviderDto { get; set; }  

    }
}