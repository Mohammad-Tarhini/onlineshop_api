using onlineshopowner_api.Application.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace onlineshopowner_api.Application.Interfaces.Ivalidator
{
    public interface IAuthHelper
    {
        Task<(bool isfound, Domain.Entities.Person person, string message)> CheckExistPersonByEmail(LoginRequestDto dto);
        Task<(bool isfound, Domain.Entities.Person person, string message)> CheckExistPersonByPhoneNumber(LoginRequestDto dto);
        Task<(bool IsSuccess, bool isfound, Domain.Entities.Person person, string message)> foundingperson(RegistrationRequestDto dto);
        void ValidateInput(RegistrationRequestDto dto);
        Task<(bool IsSuccess, bool arleadyExistClient, string message)> Registeronclient(bool arleadexistperson, Domain.Entities.Person personToProcess);
        Task<(bool IsSuccess, bool arleadyExistShopOwner, string message)> RegisterShopOwner(bool arleadexistperson, Domain.Entities.Person personToProcess);
        Task<(bool IsSuccess, bool arleadyExistadmin, string message)> Registeradmin(bool arleadexistperson, Domain.Entities.Person personToProcess);
    }
}
