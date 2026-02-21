using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;

namespace onlineshopowner_api.Domain.Entities.Delivery
{
    public class DeliveryProvider
    {
        public int Delivery_id {  get; set; }
        public string provider_type { get; set; }
        public string note_text { get; set; }

        public bool active_bit { get; set; }

        //public List<DeliveryWorkigHours> DeliveryWorkigHours { get; set; }

        //public List<string> regionname { get; set; }
        public int person_id { get; set; }

        public DateTime Create_at { get; set; }
        public decimal price_delivery_per_km { get; set; }
    }
}