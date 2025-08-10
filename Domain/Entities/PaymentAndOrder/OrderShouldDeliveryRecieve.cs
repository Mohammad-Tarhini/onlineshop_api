using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace onlineshopowner_api.Domain.Entities.PaymentAndOrder
{
    public class OrderShouldDeliveryRecieve
    {
        public int deliveryid {  get; set; }

        public int orderid { get; set; }


        public string ShopDeliveryPin{get; set; }

        public string ClientDeliveryPin {  get; set; }


    }
}