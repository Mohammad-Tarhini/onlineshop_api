using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace onlineshopowner_api.Domain.Entities.Delivery
{
    public class DeliveryWorkigHours
    {
        public string WeekDay { get; set; }

        public TimeSpan Open_time { get; set; }

        public TimeSpan Close_time { get; set; }
    }
}