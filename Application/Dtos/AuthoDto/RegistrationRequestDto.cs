using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace onlineshopowner_api.Application.Dtos
{
    public class RegistrationRequestDto
    {
        [Required(ErrorMessage = "Person details are required.")]
        public PersonDto personDto {  get; set; }

        [Required(ErrorMessage = "Role is required.")]
        [RegularExpression("^(Client|Admin|ShopOwner|Delivery)$",
            ErrorMessage = "Role must be either Client, Admin, or ShopOwner.")]
        public string role { get; set; }
    }
}