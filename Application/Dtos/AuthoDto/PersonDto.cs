using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace onlineshopowner_api.Application.Dtos
{
    public class PersonDto
    {
       
        
            [Required(ErrorMessage = "First name is required.")]
            [StringLength(50, ErrorMessage = "First name cannot exceed 50 characters.")]
            public string FirstName { get; set; }

            [Required(ErrorMessage = "Last name is required.")]
            [StringLength(50, ErrorMessage = "Last name cannot exceed 50 characters.")]
            public string LastName { get; set; }

            [Required(ErrorMessage = "Email is required.")]
            [EmailAddress(ErrorMessage = "Invalid email address.")]
            public string Email { get; set; }

            [Required(ErrorMessage = "Password is required.")]
            [StringLength(100, MinimumLength = 6, ErrorMessage = "Password must be at least 6 characters.")]
            public string Password { get; set; }

            public string Sex { get; set; }  // Could use enum if you want strict values

            [Required(ErrorMessage = "Phone number is required.")]
            [Phone(ErrorMessage = "Invalid phone number.")]
            public string PhoneNumber { get; set; }
        }

    
}