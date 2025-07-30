using onlineshopowner_api.Application.Dtos;
using onlineshopowner_api.Application.Interfaces.Iservices;
using onlineshopowner_api.Domain.Interfaces.IRepository;
using onlineshopowner_api.Infrastructure.BinderModel;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using System.Web.Http.ModelBinding;

namespace onlineshopowner_api.Controllers
{

    public class ShopController : ApiController
    {
        private readonly IshopServices shopServices;
        public ShopController(IshopServices shopServices)
        {
            this.shopServices = shopServices;
        }
        [JwtAuthorize(Roles = "shopowner")]
        [HttpPost]
        [Route("api/shop/opennewshop")]
        public async Task<IHttpActionResult> OpenNewShop([FromBody] OpenNewShopDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var (issucces, message) = await shopServices.OpenShop(dto);
            if (!issucces)
            {
                return BadRequest(message);
            }
            return Ok(issucces);

        }

        [JwtAuthorize(Roles = "shopowner")]
        [HttpPost]
        [Route("api/shop/updateprofile")]
        public async Task<IHttpActionResult> updateprofile([ModelBinder(typeof(UpdateProfileShopDtoModelBinder))] UpdatProfileShopeDto dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);

            }
            var (issucces, message) = await shopServices.PutProfileForShop(dto);
            if (!issucces) { return BadRequest(message); }
            else return Ok(message);
        }
        [HttpGet]
        [Route("api/shop/getshoptypes")]
        public async Task<IHttpActionResult> GetShopTypes([FromUri]int limit = 30,[FromUri] int page = 1, [FromUri] String search=null)
        {
            try
            {
                var (issuccess, value,currentpage,pagesize,message) = await shopServices.GetShopTypes(limit, page,search);
                if (!issuccess)
                    return BadRequest(message);
                else return Ok(new
                {
                    data = value,
                    page =currentpage,
                    limit=pagesize,
                   
                });
            }catch (Exception ex) {return BadRequest(ex.Message+"cc"); }
        }
        [HttpGet]
        [Route("api/shop/Getshops")]
        public async Task<IHttpActionResult> Getshops([FromUri] int limit = 20, [FromUri] int pagenb = 1, [FromUri] string searchbyshopname = null, [FromUri] string searchbyshoptype = null)
        {
            try
            {
                var (issuccess, shops, message) = await shopServices.GetShops(limit, pagenb, searchbyshopname, searchbyshoptype);
                if (issuccess)
                {
                    return Ok(new
                    {
                        shops = shops

                    });
                }
                else
                {
                    return BadRequest(message);
                }
            }catch (Exception ex) {return BadRequest(ex.Message); }
        }
        




    }
     
}
