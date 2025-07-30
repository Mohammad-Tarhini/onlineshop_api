using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace onlineshopowner_api.Application.Dtos
{
    public class RegisterationRequestDto
    {

        public PersonDto personDto {  get; set; }

        [Required]
        public string role { get; set; }
    }
}