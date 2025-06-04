using onlineshopowner_api.Application.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace onlineshopowner_api.Application.Interfaces.Ivalidator
{
    public interface ILoginhelper
    {
        Task<(bool isfound, Domain.Entities.Person person, string message)> CheckExistPersonByEmail(LoginRequestDto dto);
        Task<(bool isfound, Domain.Entities.Person person, string message)> CheckExistPersonByPhoneNumber(LoginRequestDto dto);
    }
}
