using onlineshopowner_api.Application.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace onlineshopowner_api.Application.Interfaces.Iservices
{
    public interface ILogin
    {
        Task<(bool IsSuccess, string Token, string error)> LoginClientOrShopowner(LoginRequestDto Dto, string role);
    }
}
