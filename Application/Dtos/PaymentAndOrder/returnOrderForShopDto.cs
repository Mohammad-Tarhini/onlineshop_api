using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace onlineshopowner_api.Application.Dtos.PaymentAndOrder
{
    public class returnOrderForShopDto
    {

        public int OrderId { get; set; }

        public decimal TotalPrice { get; set; }
        public int OrderStatus { get; set; }

        public string deliveryname { get; set; }

        public string deliveryemail { get; set; }

        public string deliveryphone { get; set; }

        public int deliveryid { get; set; }

    }
}