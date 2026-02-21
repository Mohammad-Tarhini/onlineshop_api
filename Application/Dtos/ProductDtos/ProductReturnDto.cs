using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace onlineshopowner_api.Application.Dtos.ProductDtos
{
    public class ProductReturnDto
    {
        public int Id { get; set; }
        public int shopoId { get; set; }
        public string Name { get; set; }

        public string Description { get; set; }

   
        public decimal Price { get; set; }

        public string ImgUrl { get; set; }

      
        public string Category { get; set; }

        public string Status { get; set; }

      
        public int Quantity { get; set; }


        
    }
}