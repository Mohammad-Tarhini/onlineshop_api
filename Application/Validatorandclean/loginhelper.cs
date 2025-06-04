using onlineshopowner_api.Application.Dtos;
using onlineshopowner_api.Application.Interfaces.Iservices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using onlineshopowner_api.Domain;
using Microsoft.Ajax.Utilities;

namespace onlineshopowner_api.Application.Validatorandclean
{
    public class loginhelper
    {
        private readonly IUnityOfWork _unitOfWork;
        public loginhelper(IUnityOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public async Task<(bool isfound, Domain.Entities.Person person, string message)> CheckExistPersonByEmail(LoginRequestDto dto)
        {
            Domain.Entities.Person thePerson;
            try
            {
                var thepersonE = await _unitOfWork.PersonRepository.GetPersonByEmailAsync(dto.email);
                if (!thepersonE.IsSuccess)
                {
                    return (false, null, "try enter by other way ");
                }

                thePerson = thepersonE.Value;

                if (thePerson.FirstName != dto.first_name || thePerson.LastName != dto.last_name)
                {
                    return (false, null, "the email or name are not related");
                }
                if (dto.phonenumber != null)
                {
                    if (dto.phonenumber != thePerson.PhoneNumber)
                    {
                        return (false, null, "the email or phone number are wrong ");
                    }
                }

                return (true, thePerson, "is  the person is found by email");
            }
            catch (Exception ex) { return (false, null, ex.Message); }
        }
        public async Task<(bool isfound, Domain.Entities.Person person, string message)> CheckExistPersonByPhoneNumber(LoginRequestDto dto)
        {
            Domain.Entities.Person thePerson;
            try {
                var thepesonPh = await _unitOfWork.PersonRepository.GetPersonByPhoneNumberAsync(dto.phonenumber);
                thePerson = thepesonPh.Value;
                if (!thepesonPh.IsSuccess)
                {
                    return (false, null, "this phone number is not found");
                }
                if (!thepesonPh.IsFound)
                {
                    return (false, null, "the person is not found");
                }
                if (thePerson.FirstName != dto.first_name || thePerson.LastName != dto.last_name)
                {
                    return (false, null, "the phone number or name are not related");
                }

                if (dto.email != null)
                {
                    if (thePerson.PhoneNumber != dto.phonenumber)
                    {
                        return (false, null, "the email or  phone number are you enter are wrong");
                    }
                }
                return (true, thePerson, "the person exist");
            } catch (Exception ex) { return (false, null, ex.Message); } }
       

              

    }

}