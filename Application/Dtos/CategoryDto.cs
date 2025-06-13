using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace onlineshopowner_api.Application.Dtos
{
    public class CategoryDto
    {
        [Required]
        public string name { get; set; }
    }
}