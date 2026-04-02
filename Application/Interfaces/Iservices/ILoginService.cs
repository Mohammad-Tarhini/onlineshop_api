using onlineshopowner_api.Application.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace onlineshopowner_api.Application.Interfaces.Iservices
{
    public  interface ILoginService
    {
        Task<string> Login(LoginRequestDto dto);
         Task RoleVerify(string role, int personId);
    }
}
