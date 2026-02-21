using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;


namespace onlineshopowner_api.Infrastructure.ExternalServices.googlemap
{
    public class Route
    {
        public Summary summary { get; set; }
        public string geometry { get; set; }
    }
}