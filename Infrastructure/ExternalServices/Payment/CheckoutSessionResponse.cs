using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace onlineshopowner_api.Infrastructure.ExternalServices.Payment
{
    public class CheckoutSessionResponse
    {
        public string SessionId { get; set; }
        public string CheckoutUrl { get; set; }
    }
}