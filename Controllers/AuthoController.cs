using onlineshopowner_api.Application.Dtos;
using onlineshopowner_api.Application.Interfaces.Iservices;
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
    public class AuthoController : ApiController
    {
        private readonly IAuthoServices authoServices;

        public AuthoController(IAuthoServices authoServices)
        {
            this.authoServices = authoServices;
        }
        [HttpPost]
        [Route("api/registeration/client")]
        public async Task<IHttpActionResult> registerclient([FromBody] RegisterationRequestDto dto)
        {
            string Role = dto.role.ToLower();
            if (Role != "client")
                return BadRequest();

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var (IsSuccess, already, Token, Message) = await authoServices.RegisterAsync(dto);

            if (!IsSuccess)
                return BadRequest(Message ?? "Registration failed due to unknown error.");
            if (already)
            {
                return BadRequest(Message ?? "is arleady exist");
            }
            return Ok(new { Token });
        }
        [HttpPost]
        [Route("api/registeration/shopowner")]
        public async Task<IHttpActionResult> registershopowner([FromBody] RegisterationRequestDto dto)
        {
            string Role = dto.role.ToLower();
            if (Role != "shopowner")
                return BadRequest(ModelState);

            if (!ModelState.IsValid)
                return BadRequest(ModelState);


            var (IsSuccess, already, Token, Message) = await authoServices.RegisterAsync(dto);

            if (!IsSuccess)
                return BadRequest(Message ?? "Registration failed due to unknown error.");
            if (already)
            {
                return BadRequest(Message ?? "is arleady exist");
            }
            return Ok(new { Token });

        }

        [HttpPost]
        [Route("api/registeration/admin")]
        public async Task<IHttpActionResult> registeradmin([FromBody] RegisterationRequestDto dto)
        {
            string Role = dto.role.ToLower();
            if (Role != "admin")
                return BadRequest(ModelState);

            if (!ModelState.IsValid)
                return BadRequest(ModelState);


            var (IsSuccess, already, Token, Message) = await authoServices.RegisterAsync(dto);

            if (!IsSuccess)
                return BadRequest(Message ?? "Registration failed due to unknown error.");
            if (already)
            {
                return BadRequest(Message ?? "is arleady exist");
            }
            return Ok(new { Token });

        }



        [HttpPost]
        [Route("api/Login/Client")]
        public async Task<IHttpActionResult> ClientLogin([FromBody] LoginRequestDto Dto)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);
                var (IsSuccess, token, message) = await authoServices.LoginClientOrShopownerOrAdmin(Dto, "client");
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
        [Route("api/Login/shopowner")]
        public async Task<IHttpActionResult> ShopOwnerLogin([FromBody] LoginRequestDto Dto)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);
                var (IsSuccess, token, message) = await authoServices.LoginClientOrShopownerOrAdmin(Dto, "shopowner");
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
                var (IsSuccess, token, message) = await authoServices.LoginClientOrShopownerOrAdmin(Dto, "admin");
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

