using onlineshopowner_api.Application.Dtos;
using onlineshopowner_api.Application.Interfaces.Iservices;
using onlineshopowner_api.Application.Interfaces.Ivalidator;
using onlineshopowner_api.Domain.Entities;
using onlineshopowner_api.Domain.Interfaces.IRepository;
using onlineshopowner_api.Infrastructure.ExternalServices;
using onlineshopowner_api.Infrastructure.Repositories;
using System;
using System.Collections.Generic;
using System.Diagnostics.Metrics;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.UI;

namespace onlineshopowner_api.Application.Services
{
    public class ProductServices : IProductServices
    {
        private readonly IUserContextService _usercontext;
        private readonly IUnityOfWork _unityofwork;
        private readonly IImageService _imageservice;
        private readonly IRedisRepository _redisRepository;

        public ProductServices(IUserContextService userContextService, IUnityOfWork unityOfWork,IRedisRepository redisRepository,IImageService imageService)
        {
            _usercontext = userContextService;
            _unityofwork = unityOfWork;
            _redisRepository = redisRepository;
            _imageservice = imageService;
        }

        public async Task<(bool issuccess, string message)> addproduct(ProductDto dto)
        {
            int _userId;
            string _role;
            int shopownerid;
            try
            {
                _userId = _usercontext.GetUserId();
                _role = _usercontext.GetUserRole();
            }
            catch (UnauthorizedAccessException ex)
            {
                return (false, ex.Message);
            }
            _role = _role.ToLower();
            //check if shopowner
            if (_role != "shopowner") return (false, "is not shopowner ");
            //get shopownerid
            var resultshopowner = await _unityofwork.PersonRepository.GetShopOwnerIdByPersonId(_userId);
            if (resultshopowner == null) return (false, "error");
            if (!resultshopowner.IsSuccess) { return (false, resultshopowner.Error); }
            shopownerid = resultshopowner.Value;
            //check if shopowner is the owner for this shop
            var resultshopid = await _unityofwork.ShopRepository.GetShopByShopOwnerid(shopownerid);
            if (!resultshopid.IsSuccess) return (false, resultshopid.Error);
            if (!resultshopid.IsFound) return (false, resultshopid.Error);
            if (resultshopid.Value != dto.shop_id) return (false, "you are not shopowner for this shop");
            //check if product exist in shop
            var resultproduct = await _unityofwork.ProductRepository.GetProductid(dto.name, dto.shop_id);
           
            if (!resultproduct.IsSuccess) { return (false, resultproduct.Error); }
            if (resultproduct.IsFound) return (false, "the product name is exist before ");
            var product = new Domain.Entities.Product
            {
                name = dto.name,
                shop_id = dto.shop_id,
                category_id = dto.category_id,
                description = dto.description,
                price = dto.price,


            };
            var (issuccess, message) = await _unityofwork.ProductRepository.addproduct(product);
            if (issuccess) { return (issuccess, message); }
            else { return (false, message); }




        }
        public async Task<(bool IsSuccess, string message)> AddImageProduct(AddProductImageDto dto,bool isprofile)
        {
            int userId;
            string role;

            try
            {
                userId = _usercontext.GetUserId();
                role = _usercontext.GetUserRole();
            }
            catch (UnauthorizedAccessException ex)
            {
                return (false, ex.Message);
            }
            try
            {
                if (userId == 0) return (false, "user  can not ");
                if (role == null) return (false, "error");
                role = role.ToLower();
                if (role != "shopowner") return (false, "is not shopowner");
                // Check if this user is a valid shop owner
                var personResult = await _unityofwork.PersonRepository.GetPersonByPersonId(userId);
                if (!personResult.IsSuccess || !personResult.IsFound)
                    return (false, personResult.Error);

                var shopOwnerResult = await _unityofwork.PersonRepository.GetShopOwnerByPersonAsync(personResult.Value);
                if (!shopOwnerResult.IsSuccess || !shopOwnerResult.IsFound)
                    return (false, shopOwnerResult.Error);

                var shopOwner = shopOwnerResult.Value;

                // Check if the shop belongs to the shop owner
                var shopResult = await _unityofwork.ShopRepository.GetShopByShopOwner(shopOwner);
                if (!shopResult.IsSuccess || !shopResult.IsFound)
                    return (false, shopResult.Error);

                var shop = shopResult.Value;

                if (shop.shopid != dto.shopid)
                    return (false, "You are not the owner of this shop.");
                //check get this product
                var productresult = await _unityofwork.ProductRepository.GetProductById(dto.productid);
                if (!productresult.IsSuccess) return (false, productresult.Error);
                if (!productresult.IsFound) return (false, "product is not found ");

                // Upload new image to cloud
                string logoUrl = null;
                string deleteHash = null;
            
            try
            {
                bool uploadSuccess;
                string cloudResponse;

                    if (!string.IsNullOrEmpty(dto.logo_url))
                    {
                        (uploadSuccess, cloudResponse, deleteHash) = await _imageservice.ProcessImageAsync(100, 199, 100, imageUrl: dto.logo_url);
                    }
                    else if (dto.File != null && dto.File.ContentLength > 0)
                    {
                        (uploadSuccess, cloudResponse, deleteHash) = await _imageservice.ProcessImageAsync(10000, 19900, 109990, file: dto.File);
                    }
                    else { return (false, "no file or url"); }

                if (!uploadSuccess)
                    return (false, cloudResponse);

                logoUrl = cloudResponse;
            }
            catch(Exception ex)
            {
                return (false, ex.Message+ex.InnerException);
            }
            try
            {
                var (issucces,message)=await _unityofwork.ProductRepository.AddUrlImageToProductImages(logoUrl,deleteHash,productresult.Value.product_id,isprofile);
                if (issucces) { return (true, "success"); }
                return(false,message??"hhhhhh");
            }catch(Exception ex)
            {return(false,ex.Message??"unexpected error");

            }

            }
            catch (Exception ex)
            {
                return (false, ex.Message);
            }
        }
        public async Task<(bool issucess,List<ProductDto> productDtos,string message)> GetProducts(int shopid = 0, int limit = 30, int page = 1, string searchbyproductname = null, string searchbycategory = null, string searchbyshoptype = null)
        {
            try
            {
                int allkeyscount;
                var (status, redisproductdto, message) = await _unityofwork.RedisRepository.GetProductFromRedis(shopid,limit, page, searchbyproductname, searchbycategory,searchbyshoptype);
                int offset;
                if (status == "fullsuccess")
                    return (true, redisproductdto, null);
                else
                {
                    var isint = int.TryParse(message, out allkeyscount);
                    if (isint)
                        offset = allkeyscount + (page - 1) * limit;
                    else offset = (page - 1) * limit;
                }

                var resultdb = await _unityofwork.ProductRepository.GetproducToUser(shopid,limit,offset,searchbyproductname,searchbycategory,searchbyshoptype);
                if (resultdb.IsSuccess)
                {
                    var products = resultdb.Value;
                    var productdtos=new List<ProductDto>();
                    foreach(Product  product in products)
                    {
                        var productdto = new ProductDto
                        {
                            Id = product.product_id,
                            name=product.name,
                            price = product.price,
                            description=product.description,
                            img_urlid=product.imgurid,
                            status=product.status,
                            quentity=product.quentity,
                            shop_id=product.shop_id,
                            category_id=product.category_id,
                            
                        };
                        productdtos.Add(productdto);
                        if (RedisRepository.ProductRedisCount < 50)
                        {
                            await _redisRepository.SetProductInRedis(productdto);
                            RedisRepository.ProductRedisCount++;
                        }

                    }
                   
                    
                    return (true, productdtos, "");
                }
                else
                { return (false, null, resultdb.Error); }


            }
            catch(Exception ex)
            {
                return(false,null,ex.Message);
            }
        }

}   } 