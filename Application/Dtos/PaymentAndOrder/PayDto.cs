using onlineshopowner_api.Application.Dtos.PaymentAndOrder;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace onlineshopowner_api.Application.Dtos.Payment
{
    public class PayDto
    {

        //public string paymentMethode {  get; set; }

        public List<OrderItemRequestDto> Items { get; set; }

        public OrderLocationDto OrderLocation { get; set; }
      
        public int DeliveryProviderId {  get; set; }
    }
}

