using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace onlineshopowner_api.Infrastructure.ExternalServices.Payment
{
    public class CheckoutSessionRequest
    {
        public int OrderId { get; set; }
        public decimal Amount { get; set; }
        public string Currency { get; set; } = "USD";
        public string WebhookUrl { get; set; }
    }
}