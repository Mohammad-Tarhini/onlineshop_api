using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace onlineshopowner_api.Application.Dtos
{
    public class VerifyOtpDto
    {
       
        public string email { get; set; }
        public string phoneNumber { get; set; }

        [Required]
        public string Otp { get; set; }
    }
}