using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using Newtonsoft.Json;

namespace onlineshopowner_api.Application.Dtos
{
    public class AddProductImageDto
    {
        [Required]
        public int shopid { get; set; }
        [Required]
        public int productid {  get; set; }
        public string logo_url { get; set; }


        [JsonIgnore] // Don't expect this in JSON, only multipart
        public HttpPostedFile File { get; set; }
    }
}