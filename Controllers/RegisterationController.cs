using onlineshopowner_api.Application.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using System.Web.Http;
using onlineshopowner_api.Application.Services;
using onlineshopowner_api.Application.Interfaces.Iservices;
using System.Web.UI.WebControls;


namespace onlineshopowner_api.Controllers
{
    
    public class RegisterationController : ApiController
    {
        private readonly IRegisterationServices _registrationService;
        
        public RegisterationController(IRegisterationServices registrationService)
        {
            _registrationService = registrationService;
        }
        [HttpPost]
        [Route("api/registeration/client")]
        public async Task<IHttpActionResult> registerclient([FromBody] RegisterationRequestDto dto)
        {
            string Role = dto.role.ToLower();
            if(Role != "client")
                return BadRequest();
            
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var (IsSuccess,already ,Token, Message) = await _registrationService.RegisterAsync(dto);

            if (!IsSuccess)
                return BadRequest(Message ?? "Registration failed due to unknown error.");
            if (already) {
                return BadRequest(Message?? "is arleady exist");
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
            

            var (IsSuccess, already, Token, Message) = await _registrationService.RegisterAsync(dto);

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


            var (IsSuccess, already, Token, Message) = await _registrationService.RegisterAsync(dto);

            if (!IsSuccess)
                return BadRequest(Message ?? "Registration failed due to unknown error.");
            if (already)
            {
                return BadRequest(Message ?? "is arleady exist");
            }
            return Ok(new { Token });

        }

    }
}
