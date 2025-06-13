using onlineshopowner_api.Application.Dtos;
using onlineshopowner_api.Application.Services;
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
    public class AddCategoryByAdminController : ApiController
    {
        private AddCategoryservices _AddCategoryservices { get; set; }
        public AddCategoryByAdminController(AddCategoryservices addCategoryservices)
        {
            _AddCategoryservices = addCategoryservices;
        }
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
    }
}
