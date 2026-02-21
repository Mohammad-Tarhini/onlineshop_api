using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace onlineshopowner_api.Domain.Entities.Delivery
{
    public class GetDelivery
    {
        public int Delivery_id { get; set; }
        public string provider_type { get; set; }
        public string first_name { get; set; }
        public string last_name { get; set; }
        public string phonenumber { get; set; }
        public string email { get; set; }
        public string note_text { get; set; }

        public decimal pricePerKm { get; set; }

    }
}