using onlineshopowner_api.Application.Dtos;
using onlineshopowner_api.Application.Interfaces.Iservices;
using onlineshopowner_api.Application.Interfaces.Ivalidator;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.UI.WebControls;

namespace onlineshopowner_api.Controllers
{
    [Route("api/Login/{id?}")]
    public class LoginController : ApiController
    {
        private readonly ILogin _login;
        public LoginController(ILogin login)
        {
            _login = login;
        }
        public async Task<IHttpActionResult> ClientLogin([FromBody] LoginRequestDto Dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            var (IsSuccess,token,message)=await _login.LoginClientOrShopowner(Dto,"client");
            if (!IsSuccess) {
                return BadRequest(message);
            }
            return Ok(token);


        }

        public async Task<IHttpActionResult> ShopOwnerLogin([FromBody] LoginRequestDto Dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            var (IsSuccess, token, message) = await _login.LoginClientOrShopowner(Dto, "shopowner");
            if (!IsSuccess)
            {
                return BadRequest(message);
            }
            return Ok(token);

        }
    }
}
