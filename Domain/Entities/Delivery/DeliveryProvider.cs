using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace onlineshopowner_api.Domain.Entities.Delivery
{
    public class DeliveryProvider
    {
        public string provider_type { get; set; }
        public string note_text { get; set; }

        public bool active_bit { get; set; }

        public List<DeliveryWorkigHours> DeliveryWorkigHours { get; set; }

        public List<string> regionname { get; set; }
    }
}