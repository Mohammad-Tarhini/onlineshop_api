using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using onlineshopowner_api.Domain.Interfaces;

namespace onlineshopowner_api.Infrastructure.MappingDomainModel
{
    public  class PersonMapper : IMapper<Domain.Entities.Person, Models.Person>
    {
        public  Models.Person ToEntity(Domain.Entities.Person person)
        {
            return new Models.Person
            {
                person_id=person.PersonId,
                first_name = person.FirstName,
                last_name = person.LastName,
                email = person.Email,
                password = person.Password,
                sex = person.Sex,
                phone_number = person.PhoneNumber,
                created_date=person.CreatedDate,
            };
        }
        public Domain.Entities.Person ToDomain(Models.Person person)
        {
            return new Domain.Entities.Person
            {
                PersonId=person.person_id,
                FirstName=person.first_name,
                LastName=person.last_name,
                Email=person.email,
                Sex=person.sex,
                Password=person.password,
                PhoneNumber=person.phone_number,
                CreatedDate=person.created_date ?? DateTime.UtcNow,
                

                

            };



        }
    }
}