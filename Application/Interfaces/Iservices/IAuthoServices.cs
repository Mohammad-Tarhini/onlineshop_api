using onlineshopowner_api.Application.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace onlineshopowner_api.Application.Interfaces.Iservices
{
    public interface IAuthoServices
    {
        Task<(bool IsSuccess, string Token, string error)> LoginClientOrShopownerOrAdmin(LoginRequestDto Dto, string role);
        Task<(bool IsSuccess, bool alreadyexist, string Token, string Message)> RegisterAsync(RegisterationRequestDto dto);
    }
}
