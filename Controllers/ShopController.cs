using onlineshopowner_api.Application.Dtos;
using onlineshopowner_api.Application.Interfaces.Iservices;
using onlineshopowner_api.Domain.Interfaces.IRepository;
using onlineshopowner_api.Infrastructure.BinderModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.ModelBinding;

namespace onlineshopowner_api.Controllers
{
    
    public class ShopController : ApiController
    {
        private readonly IOpenNewShopServices _OpenNewShopServices;
        private readonly IUpdateProfileShop _updateprofile;
        public ShopController(IOpenNewShopServices opennewshopservices,IUpdateProfileShop updateprofile)
        {
          _OpenNewShopServices = opennewshopservices;
            _updateprofile = updateprofile;
        }
        [HttpPost]
        [Route("api/shop/opennewshop")]
        public async Task<IHttpActionResult> OpenNewShop([FromBody] OpenNewShopDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            
            var (issucces,message)=await _OpenNewShopServices.OpenShop(dto);
            if (!issucces)
            {
                return BadRequest(message) ;
            }
            return Ok(issucces);

        }
        [HttpPost]
        [Route("api/shop/updateprofile")]
        public  async Task<IHttpActionResult> updateprofile([ModelBinder(typeof(UpdateProfileShopDtoModelBinder))] UpdatProfileShopeDto dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);

            }
            var(issucces,message)=await _updateprofile.PutProfileForShop(dto);
            if (!issucces) { return BadRequest(message); }
            else return Ok(message);
        }


    }
     
}
