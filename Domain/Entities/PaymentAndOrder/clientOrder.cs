using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace onlineshopowner_api.Domain.Entities.PaymentAndOrder
{
    public class clientOrder
    {
        public int orderId { get; set; }
        public int clientId { get; set; }
        public decimal totalPrice { get; set; }

        public DateTime orderDate { get; set; }
        public string orderStatus { get; set; }
        public int shopId { get; set; }

        public string shopdeliverypin { get; set; }
        public string clientdeliverypin { get; set; }

        public int deliveryProviderId { get; set; }

        public decimal latitude { get; set; }
        public decimal longitude { get; set; }

        public decimal ProductTotalCost { get; set; }
        public decimal DeliveryCost { get; set; }

    }
}