using onlineshopowner_api.Application.Dtos;
using onlineshopowner_api.Application.Interfaces.Iservices;
using onlineshopowner_api.Application.ModelBinders;
using onlineshopowner_api.Infrastructure.BinderModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.ModelBinding;
using System.Web.UI;

namespace onlineshopowner_api.Controllers
{
    public class ProductController : ApiController
    {
        private readonly IProductServices _productservices;
        public ProductController(IProductServices productservices)
        {
            _productservices = productservices;
        }
        //products
        [JwtAuthorize(Roles = "shopowner")]
        [HttpPost]
        [Route("api/product/addproduct")]
        public async Task<IHttpActionResult> AddProduct()
        {
          var request = System.Web.HttpContext.Current.Request;
            var dto= new ProductRequestDto
            {
                Name = request.Form["Name"],
                Description = request.Form["Description"],
                Price = decimal.TryParse(request.Form["Price"], out var price) ? price : 0,
                CategoryId = int.TryParse(request.Form["CategoryId"], out var categoryId) ? categoryId : 0,
                Quantity = int.TryParse(request.Form["Quantity"], out var quantity) ? quantity : 0,
                File = request.Files.Count > 0 ? request.Files[0] : null,
                ImgUrl=request.Form["ImgUrl"],
                Status=request.Form["Status"],

            };

            await _productservices.AddProductByShopOwner(dto);
            return Ok("is added");

        }
        public async Task<IHttpActionResult> UpdateProduct()
        {
            var request = System.Web.HttpContext.Current.Request;
            var dto = new ProductRequestDto
            {
                Name = request.Form["Name"],
                Description = request.Form["Description"],
                Price = decimal.TryParse(request.Form["Price"], out var price) ? price : 0,
                CategoryId = int.TryParse(request.Form["CategoryId"], out var categoryId) ? categoryId : 0,
                Quantity = int.TryParse(request.Form["Quantity"], out var quantity) ? quantity : 0,
                File = request.Files.Count > 0 ? request.Files[0] : null,
                ImgUrl = request.Form["ImgUrl"],
                Status = request.Form["Status"],
            };
            await _productservices.UpdateProduct(dto);
            return Ok("is updated");
        }
        public async Task<IHttpActionResult> Getproduct(int shopid = 0, int limit = 30, int page = 1, string searchbyproductname = null, string searchbycategory = null, string searchbyshoptype = null)
        {
            var (products, Rlimit, Rpage) = await _productservices.GetProducts(shopid, limit, page, searchbyproductname, searchbycategory, searchbyshoptype);
            return Ok(new { products, Rlimit, Rpage });
        }


        //    [JwtAuthorize(Roles = "shopowner")]
        //    [HttpPost]
        //    [Route("api/product/updateProduct")]
        //    public async Task<IHttpActionResult> UpdateProduct()
        //    {
        //        try
        //        {
        //            if (!ModelState.IsValid) { return BadRequest(ModelState); }
        //            var (issuccess, message) = await _productservices.AddImageProduct(dto, isprofile);
        //            if (issuccess) { return Ok("is added"); }
        //            if(!issuccess)
        //            return BadRequest(message ?? "rrrr");
        //            return BadRequest("NOOOO");
        //        }
        //        catch (Exception ex) { return BadRequest(ex.Message); }
        //    }
        //    [HttpGet]
        //    [Route("api/product/getproducts")]
        //    public async Task<IHttpActionResult> Getproduct(int shopid = 0, int limit = 30, int page = 1, string searchbyproductname = null, string searchbycategory = null, string searchbyshoptype = null)
        //    {
        //        try
        //        {
        //            var (issuccess,products,message)=await _productservices.GetProducts(shopid,limit,page,searchbyproductname,searchbycategory,searchbyshoptype);
        //            if (issuccess) { return Ok(products); }
        //            else { return BadRequest(message); }
        //        }
        //        catch (Exception ex)
        //        {
        //            return BadRequest(ex.Message);
        //        }
        //    }
        //
    }
}
