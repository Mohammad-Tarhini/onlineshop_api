using onlineshopowner_api.Application.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace onlineshopowner_api.Application.Interfaces.Iservices
{
    public interface IRegisterationServices
    {
        Task<(bool IsSuccess, bool alreadyexist, string Token, string Message)> RegisterAsync(RegisterationRequestDto dto);
    }
}
