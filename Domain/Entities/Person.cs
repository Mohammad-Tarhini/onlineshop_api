using onlineshopowner_api.Infrastructure.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace onlineshopowner_api.Domain.Entities
{
    public class Person
    {
        public int PersonId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string Sex { get; set; }
        public string PhoneNumber { get; set; }
        public DateTime CreatedDate { get; set; }

        public Person(string firstname, string lastname, string email, string sex, string phonenumber, string password)
        {
            this.FirstName = firstname;
            this.LastName = lastname;
            this.Email = email;
            this.PhoneNumber = phonenumber;
            this.Sex = sex;
            this.Password = password;
            this.CreatedDate= DateTime.Now;

        }
        public Person(int personid, string firstname, string lastname, string email, string sex, string phonenumber, string password)
    : this(firstname, lastname, email, sex, phonenumber, password)
        {
            PersonId = personid;
        }
        public Person() { }
    } 
}