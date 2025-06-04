using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace onlineshopowner_api.Application.Dtos
{
    public class LoginRequestDto
    {
        [Required]
        public string first_name { get; set; }
        [Required]
        public string last_name { get; set; }
        [Required]
        public string password { get; set; }
        public string email { get; set; }
        public string phonenumber {  get; set; }

       
    }
}