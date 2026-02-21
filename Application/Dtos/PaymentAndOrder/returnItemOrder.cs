using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace onlineshopowner_api.Application.Dtos.PaymentAndOrder
{
    public class returnItemOrder
    {
        public int orderDetailId { get; set; }
        public int productId { get; set; }
        public string productName { get; set; }
        public int quantity { get; set; }
        public decimal price { get; set; }

    }
}