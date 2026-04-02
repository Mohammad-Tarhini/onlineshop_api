using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using Newtonsoft.Json;
namespace onlineshopowner_api.Application.Dtos
{
    public class ProductRequestDto
    {
        public int Id { get; set; }


        [Required]
            [StringLength(200)]
            public string Name { get; set; }

            [Required]
            [StringLength(2000)]
            public string Description { get; set; }

            [Required]
            [Range(0.01, double.MaxValue)]
            public decimal Price { get; set; }

            public string ImgUrl { get; set; }

            [Required]
            [Range(1, int.MaxValue)]
            public int CategoryId { get; set; }

            [StringLength(50)]
            public string Status { get; set; }

            [Range(0, int.MaxValue)]
            public int Quantity { get; set; }


            // Only from multipart/form-data
            [JsonIgnore]
            public HttpPostedFile File { get; set; }
        }

    
}