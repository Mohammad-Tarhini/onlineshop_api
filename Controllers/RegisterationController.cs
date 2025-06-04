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
        [Route("api/registeration")]
        public async Task<IHttpActionResult> register([FromBody] RegisterationRequestDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var (IsSuccess,already ,Token, Message) = await _registrationService.RegisterAsync(dto);

            if (!IsSuccess)
                return BadRequest(Message ?? "Registration failed due to unknown error.");
            if (already) { 
                return NotFound();
            }
            return Ok(new { Token });
        }

    }
}
