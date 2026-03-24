using onlineshopowner_api.Application.Dtos;
using onlineshopowner_api.Application.Interfaces.Iservices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;

namespace onlineshopowner_api.Controllers.AuthoControllers
{
    public class LoginController : ApiController
    {
        private readonly ILoginService loginService;

        public LoginController(ILoginService loginService)
        {
            this.loginService = loginService;
        }

        [HttpPost]
        [Route("api/Login")]
        public async Task<IHttpActionResult> Login([FromBody] LoginRequestDto dto)
        {
            
             var result=   await loginService.Login(dto);
                return Ok(result);
            
            
        }

    }
}
