using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace onlineshopowner_api.Domain.Entities.Delivery
{
    public class DeliveryPersons
    {
        public DeliveryProvider DeliveryProvider { get; set; }
        public  Person Person { get; set; }

        
    }
}