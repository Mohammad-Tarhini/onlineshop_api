using onlineshopowner_api.Application.Dtos;
using onlineshopowner_api.Application.Services;
using onlineshopowner_api.Domain.Interfaces.IExternalServices;
using onlineshopowner_api.Infrastructure.ExternalServices.onlineshopowner_api.Infrastructure.ExternalServices;
using onlineshopowner_api.Infrastructure.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;

namespace onlineshopowner_api.Controllers
{
    public class AdminController : ApiController
    {
        private AddCategoryservices _AddCategoryservices { get; set; }
        private IRedisCacheService _redisCacheService;
        public AdminController(AddCategoryservices addCategoryservices, IRedisCacheService redisCacheService)
        {
            _AddCategoryservices = addCategoryservices;
            _redisCacheService = redisCacheService;

        }
        [JwtAuthorize(Roles = "admin")]
        [HttpPost]
        [Route("api/Addcategory")]
        public async Task<IHttpActionResult> AddCategory([FromBody] List<CategoryDto> categoryDtos)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            var (isadmain, message) = await _AddCategoryservices.checkIfUserIsAdmin();
            if (!isadmain) { return BadRequest(message); }
            var (issuccess, message2) = await _AddCategoryservices.addallcategory(categoryDtos);
            if (!issuccess) { return BadRequest(message2); }
            return Ok(message2 ??" string.Empty");
        }





     /*   [HttpGet]
        [Route("api/cache/test")]
        public async Task<IHttpActionResult> TestRedis()
        {
            var cachedValue = await _redisCacheService.GetAsync("testkey");
            if (string.IsNullOrEmpty(cachedValue))
            {
                await _redisCacheService.SetAsync("testkey", "Hello from Redis");
                return Ok("Key was not found. Set now.");
            }

            return Ok("Found in Redis: " + cachedValue);
        }*/



    }
}
