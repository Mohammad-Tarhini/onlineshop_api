using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace onlineshopowner_api.Application.Dtos
{
    public class ProductDto
    {
        public int Id { get; set; }
        [Required]
        public string name { get; set; }
        [Required]
        public string description { get; set; }
        [Required]
        public decimal price { get; set; }

        public int img_urlid { get; set; }
        [Required]
        public int category_id { get; set; }

        public string status { get; set; }

        public int quentity { get; set; }
        [Required]
        public int shop_id { get; set; }

        public string shop_type { get; set; }
        public string category { get; set; }
    }
}