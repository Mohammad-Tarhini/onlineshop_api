using onlineshopowner_api.Application.Dtos;
using onlineshopowner_api.Application.Interfaces.Iservices;
using onlineshopowner_api.Domain.Interfaces.IRepository;
using System;
using System.Collections.Generic;
using System.Data.Entity.Core.Metadata.Edm;
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
        public async Task<IHttpActionResult> OpenNewShop()
        {
            var request = HttpContext.Current.Request;

            var openNewShopDto = new OpenNewShopDto
            {
                Name = request.Form["Name"],
                Description = request.Form["Description"],
                logo_url = string.IsNullOrEmpty(request.Form["logo_url"]) ? null : request.Form["logo_url"],
                Categories = string.IsNullOrEmpty(request.Form["Categories"])
                    ? new List<int>()
                    : request.Form["Categories"].Split(',')
                          .Where(s => int.TryParse(s, out _))
                          .Select(int.Parse)
                          .ToList(),
                File = request.Files.Count > 0 ? request.Files["File"] : null,
                Latitude = request.Form["Latitude"] != null && decimal.TryParse(request.Form["Latitude"], out var lat) ? lat : 0,
                Longitude = request.Form["Longitude"] != null && decimal.TryParse(request.Form["Longitude"], out var lng) ? lng : 0

            };

            string response;
            try
            {
                response = await shopServices.OpenShop(openNewShopDto);
            }
            catch (Exception ex)
            {
                return BadRequest(
                 ex.Message +ex.InnerException?.Message
                );
            }

            return Ok(response);
        }
        [JwtAuthorize(Roles = "shopowner")]
        [HttpPost]
        [Route("api/shop/updateshop")]
        public async Task<IHttpActionResult> updataShop()
        {
            var request = HttpContext.Current.Request;
            var updateShopDto = new OpenNewShopDto
            {
                Name = request.Form["Name"],
                Description = request.Form["Description"],
                Categories = request.Form["Categories"].Split(',').Select(int.Parse).ToList(),
                logo_url = request.Form["logo_url"],
                File = request.Files["File"],
                Latitude=request.Form["Latitude"]!=null?decimal.Parse(request.Form["Latitude"]):0,
                Longitude=request.Form["Longitude"]!=null?decimal.Parse(request.Form["Longitude"]): 0

            };
            string response = await shopServices.updataShop(updateShopDto);
            if (response == "success")
            {
                return Ok(response);
            }
            else
            {
                return BadRequest(response);
            }
        }
        [HttpGet]
        [Route("api/shop/Getshops")]
        public async Task<IHttpActionResult> Getshops([FromUri] int limit = 20, [FromUri] int pagenb = 1, [FromUri] string searchbyshopname = null, [FromUri] string searchbyshoptype = null)
        {
          (var shopSumaryDtos ,var limt, var page )  =await shopServices.GetShops(limit, pagenb, searchbyshopname, searchbyshoptype);
            if (shopSumaryDtos==null)
            {
                return BadRequest("new data to return");
            }
            return Ok(new
            {
                shops = shopSumaryDtos,
                limit = limt,
                page = page
            });

        }
        [HttpGet]
        [Route("api/shop/getshoptypes")]
        public async Task<IHttpActionResult> GetShopTypes([FromUri] int limit = 30, [FromUri] int page = 1, [FromUri] string search = null)
        {
            var (Types, Page, PageSize, Message) = await shopServices.GetShopTypes(limit, page, search);
            if (Types == null)
            {
                return BadRequest(Message);
            }
            else
            {
                return Ok(new
                {
                    Types = Types,
                    Page = Page,
                    PageSize = PageSize,
                    Message = Message
                });
            }
        }
        //[JwtAuthorize(Roles = "shopowner")]
        //[HttpPost]
        //[Route("api/shop/opennewshop")]
        //public async Task<IHttpActionResult> OpenNewShop([FromBody] OpenNewShopDto dto)
        //{
        //    if (!ModelState.IsValid)
        //        return BadRequest(ModelState);

        //    var (issucces, message) = await shopServices.OpenShop(dto);
        //    if (!issucces)
        //    {
        //        return BadRequest(message);
        //    }
        //    return Ok(issucces);

        //}

        //[JwtAuthorize(Roles = "shopowner")]
        //[HttpPost]
        //[Route("api/shop/updateprofile")]
        //public async Task<IHttpActionResult> updateprofile([ModelBinder(typeof(UpdateProfileShopDtoModelBinder))] UpdatProfileShopeDto dto)
        //{
        //    if (!ModelState.IsValid)
        //    {
        //        return BadRequest(ModelState);

        //    }
        //    var (issucces, message) = await shopServices.PutProfileForShop(dto);
        //    if (!issucces) { return BadRequest(message); }
        //    else return Ok(message);
        //}
        //[HttpGet]
        //[Route("api/shop/getshoptypes")]
        //public async Task<IHttpActionResult> GetShopTypes([FromUri]int limit = 30,[FromUri] int page = 1, [FromUri] String search=null)
        //{
        //    try
        //    {
        //        var (issuccess, value,currentpage,pagesize,message) = await shopServices.GetShopTypes(limit, page,search);
        //        if (!issuccess)
        //            return BadRequest(message);
        //        else return Ok(new
        //        {
        //            data = value,
        //            page =currentpage,
        //            limit=pagesize,

        //        });
        //    }catch (Exception ex) {return BadRequest(ex.Message+"cc"); }
        //}
        //[HttpGet]
        //[Route("api/shop/Getshops")]
        //public async Task<IHttpActionResult> Getshops([FromUri] int limit = 20, [FromUri] int pagenb = 1, [FromUri] string searchbyshopname = null, [FromUri] string searchbyshoptype = null)
        //{
        //    try
        //    {
        //        var (issuccess, shops, message) = await shopServices.GetShops(limit, pagenb, searchbyshopname, searchbyshoptype);
        //        if (issuccess)
        //        {
        //            return Ok(

        //                 shops

        //            );
        //        }
        //        else
        //        {
        //            return BadRequest(message+"t");
        //        }
        //    }catch (Exception ex) {return BadRequest(ex.Message); }
        //}





    }
     
}
