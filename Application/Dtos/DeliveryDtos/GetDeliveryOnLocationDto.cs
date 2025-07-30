using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace onlineshopowner_api.Application.Dtos.DeliveryDtos
{
    public class GetDeliveryOnLocationDto
    {
        public string name {  get; set; }

        public string phonenumber {  get; set; }

        public string email {  get; set; }

       public DeliveryProviderDto deliveryProviderDto { get; set; }

        public List<DeliveryWorkingHourDto> deliveryWorkingHourDto { get; set; }


    }
}