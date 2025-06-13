using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Newtonsoft.Json;

namespace onlineshopowner_api.Application.Dtos
{
    public class UpdatProfileShopeDto
    {
        public int shopid { get; set; }
        public string logo_url { get; set; }


        [JsonIgnore] // Don't expect this in JSON, only multipart
        public HttpPostedFile File { get; set; }
    }
}