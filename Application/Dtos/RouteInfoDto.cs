using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace onlineshopowner_api.Application.Dtos
{
    public class RouteInfoDto
    {
        public string Distance { get; set; }        // e.g. "3.4 km"
        public string Duration { get; set; }        // e.g. "5 mins"
        public string EncodedPolyline { get; set; } // for drawing path
    }
}