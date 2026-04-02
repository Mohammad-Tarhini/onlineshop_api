using onlineshopowner_api.Application.Dtos;
using onlineshopowner_api.Application.Dtos.AuthoDto;
using onlineshopowner_api.Application.Dtos.DeliveryDtos;
using onlineshopowner_api.Application.Interfaces.Iservices;
using onlineshopowner_api.Application.Services;
using onlineshopowner_api.Application.Services.AuthoServices;
using onlineshopowner_api.Application.Validatorandclean;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;

namespace onlineshopowner_api.Controllers.AuthoControllers
{
    public class RegisterationController : ApiController
    {

        private readonly IRegisterationServices RegisterationService;
        public RegisterationController(IRegisterationServices registerationService)
        {
            this.RegisterationService = registerationService;
        }

        [HttpPost]
        [Route("api/Client/Registeration")]
        public async Task<IHttpActionResult> register([FromBody] RegistrationRequestDto dto)
        {
            await RegisterationService.RegisterClient(dto);

            return Ok("go and check your email or phonenumber");


        }
        [HttpGet]
        [Route("api/Client/VerifyRegisteration")]
        public async Task<IHttpActionResult> verifyregisteration([FromUri] string token)
        {

            await RegisterationService.VerifyRegisteration(token);
            return Ok("verification is succesfull");

        }

        [HttpPost]
        [Route("api/ShopownerOrDelivery/Registration")]
        public async Task<IHttpActionResult> Registeration([FromBody] RegistrationRequestDto dto)
        {
            await RegisterationService.RegisterShopOwnerorDelivery(dto);
            return Ok("you will reach message go and check it ");
        }

        [HttpGet]
        [Route("api/Shopowner/VerifyShopownerRegisteration")]

        public async Task<IHttpActionResult> VerifyRegisteration([FromUri] VerifyOtpDto dto)
        {
            await RegisterationService.AddVerifiedShopowner(dto);
            return Ok("");
        }
        [HttpGet]
        [Route("api/Delivery/VerifyRegisteration")]
        public async Task<IHttpActionResult> VerifyDeliveryRegisteration([FromUri] VerifyDeliveryRequestDto dto)
        {
            await RegisterationService.AddVerifiedDelivery(dto.DeliveryProviderDto,dto.VerifyOtpDto);
            return Ok("");


        }

        [HttpPost]
        [Route("api/Testing")]
        public IHttpActionResult TestHash(string password)
        {
            var hash = HashingPassword.HashPassword(password);

            bool verify = HashingPassword.VerifyPassword(password, hash);

            return Ok(new
            {
                Password = password,
                Hash = hash,
                Verification = verify
            });
        }
    }
}
