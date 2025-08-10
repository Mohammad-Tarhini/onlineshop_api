using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace onlineshopowner_api.Application.Dtos.PaymentAndOrder
{
    public class RecievefromShopToDelivery
    {
        public string password {  get; set; }

        public string orderId { get; set; }

        public string deliveryid {  get; set; }

    }
}