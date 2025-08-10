using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace onlineshopowner_api.Domain.Entities.Delivery
{
    public class DeliveryPersons
    {
        public int Delivery_Person_Id { get; set; }
        public Person Person { get; set; }

        public DeliveryProvider deliveryprovider { get; set; }


    }
}