using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace onlineshopowner_api.Application.Dtos.Payment
{
    public class PayDto
    {

        public string paymentmethode {  get; set; }

        public List<CartItemRequestDto> Items { get; set; }

        public OrderLocationDto OrderLocation { get; set; }

        public int DeliveryProviderId {  get; set; }
    }
}

