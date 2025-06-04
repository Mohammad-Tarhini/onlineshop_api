using onlineshopowner_api.Application.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using onlineshopowner_api;


namespace onlineshopowner_api.Application.Interfaces.Ivalidator
{
    public interface Iregistrationhelper
    {
       Task<(bool IsSuccess, bool isfound, Domain.Entities.Person person, string message)> foundingperson(RegisterationRequestDto dto);
         void ValidateInput(RegisterationRequestDto dto);
        Task<(bool IsSuccess, bool arleadyExistClient , string message)> Registeronclient(bool arleadexistperson, Domain.Entities.Person personToProcess);
        Task<(bool IsSuccess,bool arleadyExistShopOwner, string message)> RegisterShopOwner(bool arleadexistperson, Domain.Entities.Person personToProcess);
    }
}

