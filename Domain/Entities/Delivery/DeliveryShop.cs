using onlineshopowner_api.Application.Dtos.DeliveryDtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace onlineshopowner_api.Domain.Entities.Delivery
{
    public class DeliveryShop
    {
        public int Shop_id { get; set; }

        public DeliveryProvider deliveryProvider { get; set; }
    }
}