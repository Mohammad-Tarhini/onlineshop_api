using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace onlineshopowner_api.Application.Dtos.DeliveryDtos
{
    public class DeliveryAgentDto
    {
        public string name {  get; set; }

        public string phone_number {  get; set; }

        public string email { get; set; }

        public string password { get; set; }

        public DeliveryProviderDto deliveryproviderdto { get; set; }//i have to rename deliveryproviderdto
        
    }
}