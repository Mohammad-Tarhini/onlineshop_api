using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;


namespace onlineshopowner_api.Infrastructure.ExternalServices.googlemap
{
    public class Route
    {
        public List<Leg> legs { get; set; }
        public OverviewPoyline overview_polyline { get; set; }
    }
}