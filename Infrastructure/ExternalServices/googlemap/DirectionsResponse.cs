using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace onlineshopowner_api.Infrastructure.ExternalServices.googlemap
{
    public class DirectionsResponse
    {
        public List<Route> routes { get; set; }
        public string status { get; set; }
    }
}