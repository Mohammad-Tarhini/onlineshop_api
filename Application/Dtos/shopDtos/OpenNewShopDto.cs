using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
 using Newtonsoft.Json;


namespace onlineshopowner_api.Application.Dtos
{
    public class OpenNewShopDto
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public string logo_url { get; set; }

        public List<int> Categories { get; set; }
        public decimal Latitude { get; set; }
        public decimal Longitude { get; set; }

        [JsonIgnore] // Don't expect this in JSON, only multipart
        public HttpPostedFile File { get; set; }




    }
}