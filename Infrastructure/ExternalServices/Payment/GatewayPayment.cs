using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace onlineshopowner_api.Infrastructure.ExternalServices.Payment
{
    public class GatewayPayment
    {
        public string SessionId { get; set; }
        public int OrderId { get; set; }
        public decimal Amount { get; set; }
        public string Status { get; set; } // Pending, Paid, Failed
        public string WebhookUrl { get; set; }
    }
}