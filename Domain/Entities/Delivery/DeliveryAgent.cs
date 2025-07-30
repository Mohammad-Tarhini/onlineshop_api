using onlineshopowner_api.Application.Dtos.DeliveryDtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace onlineshopowner_api.Domain.Entities.Delivery
{
    public class DeliveryAgent
    {
        public string name { get; set; }

        public string phone_number { get; set; }

        public string email { get; set; }

        public string password { get; set; }

        public DeliveryProvider deliveryprovider { get; set; }
    }
}