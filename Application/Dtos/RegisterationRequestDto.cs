using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace onlineshopowner_api.Application.Dtos
{
    public class RegisterationRequestDto
    {

        [Required]
        public string first_name { get; set; }
        [Required]
        public string last_name { get; set; }
        [EmailAddress]
        public string email { get; set; }
        [Required]
        public string password { get; set; }
        public string sex { get; set; }

        public string phonenumber { get; set; }

        [Required]
        public string role { get; set; }
    }
}