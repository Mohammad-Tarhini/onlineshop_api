using onlineshopowner_api.Application.Dtos;
using onlineshopowner_api.Application.Interfaces.Iservices;
using onlineshopowner_api.Application.Interfaces.Ivalidator;
using onlineshopowner_api.Infrastructure.ExternalServices;
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
   
    public class LoginController : ApiController
    {
        private readonly ILogin _login;

        public LoginController(ILogin login)
        {
            _login = login;
        }
        [HttpPost]
        [Route("api/Login/Client")]
        public async Task<IHttpActionResult> ClientLogin([FromBody] LoginRequestDto Dto)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);
                var (IsSuccess, token, message) = await _login.LoginClientOrShopownerOrAdmin(Dto, "client");
                if (!IsSuccess)
                {
                    return BadRequest(message);
                }
               

                return Ok(token);
            }
            catch (Exception ex) {
                return BadRequest(ex.ToString());
            }
        }
        [HttpPost]
        [Route("api/Login/shopowner")]
        public async Task<IHttpActionResult> ShopOwnerLogin([FromBody] LoginRequestDto Dto)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);
                var (IsSuccess, token, message) = await _login.LoginClientOrShopownerOrAdmin(Dto, "shopowner");
                if (!IsSuccess)
                {
                    return BadRequest(message);
                }
                return Ok(token);

            }
            catch (Exception ex) 
            {
                return BadRequest(ex.ToString());
            }
            }

        [HttpPost]
        [Route("api/Login/admin")]
        public async Task<IHttpActionResult> AdminLogin([FromBody] LoginRequestDto Dto)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);
                var (IsSuccess, token, message) = await _login.LoginClientOrShopownerOrAdmin(Dto, "admin");
                if (!IsSuccess)
                {
                    return BadRequest(message);
                }
                return Ok(token);

            }
            catch (Exception ex)
            {
                return BadRequest(ex.ToString());
            }
        }
    }
}
