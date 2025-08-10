using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace onlineshopowner_api.Domain.Entities.Delivery
{
    public class LoginDelivery
    {

        public string email { get; set; }

        public string phonenumber { get; set; }

        public string password { get; set; }

        public string deliverytype {  get; set; }
    }
}