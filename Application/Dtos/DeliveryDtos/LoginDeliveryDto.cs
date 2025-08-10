using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace onlineshopowner_api.Application.Dtos.DeliveryDtos
{
    public class LoginDeliveryDto
    {

        public string email { get; set; }

        public string phonenumber { get; set; }

        public  string password {  get; set; }

        public string deliverytype {  get; set; }

    }
}