using onlineshopowner_api.Application.Dtos;
using onlineshopowner_api.Application.Interfaces.Iservices;
using onlineshopowner_api.Application.Interfaces.Ivalidator;
using onlineshopowner_api.Application.Validatorandclean;
using onlineshopowner_api.Domain.Entities;
using onlineshopowner_api.Domain.Interfaces.IExternalServices;
using onlineshopowner_api.Domain.Interfaces.IRepository;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;
using System.Web;
using System.Web.Caching;
using System.Web.Http;
using System.Xml.Linq;
using Twilio.TwiML.Messaging;
using WebGrease;

namespace onlineshopowner_api.Application.Services
{
    public class ShopServices : IshopServices
    {
        private readonly IUserContextService _usercontext;
        private readonly IUnityOfWork _Unityofwork;
        private readonly IRedisCacheService _rediscache;
        private readonly IRedisRepository _redisRepository;
        private readonly IImageService _imageservice;
        private readonly IImgur _imgur;
        private int _userId;
        private string _role;




        public ShopServices(IUserContextService usercontext, IUnityOfWork unityofwork, IImageService imageservice, IImgur imgur, IRedisCacheService redisCacheService, IRedisRepository redisRepository)
        {
            _usercontext = usercontext;
            _Unityofwork = unityofwork;
            _imageservice = imageservice;
            _rediscache = redisCacheService;
            _imgur = imgur;
            _redisRepository = redisRepository;
        }
        public async Task<int> validateShopOwner()
        {
            _userId = _usercontext.GetUserId();
            _role = _usercontext.GetUserRole();
            if (_role != "shopowner")
            {
                throw new UnauthorizedAccessException("Only shop owners can open a shop.");
            }


            //check if userid is shopowner
            var shownerId = await _Unityofwork.PersonRepository.GetShopOwnerIdByPersonId(_userId);
            if (shownerId == null || shownerId == 0)
            {
                throw new UnauthorizedAccessException("User is not a shop owner.");
            }
            return shownerId.Value;
        }
        public async Task<string> updataShop(OpenNewShopDto dto)
        {
            int shopOwnerId = await validateShopOwner();
            if (shopOwnerId == null || shopOwnerId == 0)
            {
                throw new Exception("You don't have a shop to update.");
            }
            var shop = await _Unityofwork.ShopRepository.GetShopByShopOwnerIdOrShopId(shopOwnerId);
            if (shop == null)
            {
                throw new Exception("You don't have a shop to update.");
            }
            if (string.IsNullOrEmpty(dto.logo_url) && dto.File == null && string.IsNullOrEmpty(dto.Name) && string.IsNullOrEmpty(dto.Description))
            {
                throw new Exception("No data to update.");
            }
            string oldDeleteHash = null;
            if (dto.File != null || dto.logo_url != null)
            {
                (var ImageUrl, var deleteUrl) = await _imageservice.ProcessImageAsync(100, 199, 100, imageUrl: dto.logo_url, file: dto.File);

                if (ImageUrl == null && deleteUrl == null)
                {
                    throw new Exception("Failed to process image.");
                }
                //delete old image
                //if (!string.IsNullOrEmpty(shop.deletehashingimage))
                //{
                //    await _imgur.DeleteImageAsync(shop.deletehashingimage);
                //}
                oldDeleteHash = shop.deletehashingimage;
                shop.logoUrl = ImageUrl;
                shop.deletehashingimage = deleteUrl;
            }
            if (!string.IsNullOrEmpty(dto.Name))
            {
                shop.name = dto.Name;
            }
            if (!string.IsNullOrEmpty(dto.Description))
            {
                shop.description = dto.Description;
            }
            try
            {
                await _Unityofwork.ShopRepository.updateShop(shop);
            }
            catch (Exception ex)
            {
                throw new Exception("Failed to update shop: " + ex.Message);
            }
            if (!string.IsNullOrEmpty(oldDeleteHash))
            {
                await _imgur.DeleteImageAsync(oldDeleteHash);
            }
            return "Shop updated successfully.";
        }
        public async Task<string> OpenShop(OpenNewShopDto dto)
        {

            _userId = _usercontext.GetUserId();
            _role = _usercontext.GetUserRole();
            if (_role.ToLower().Trim() != "shopowner")
            {
                throw new UnauthorizedAccessException("Only shop owners can open a shop.");
            }


            //check if userid is shopowner
            var shownerId = await _Unityofwork.PersonRepository.GetShopOwnerIdByPersonId(_userId);
            if (shownerId == null || shownerId == 0)
            {
                throw new UnauthorizedAccessException("User is not a shop owner.");
            }

            var shopId = await _Unityofwork.ShopRepository.GetShopIDByShopownerId(shownerId.Value);
            if (shopId != null )
            {
                throw new Exception("You already have a shop.");
            }
            string ImageUrl = null;
            string deleteUrl = null;
            //if(dto.File==null && dto.logo_url == null)
            //{

            //}
            if (dto.File == null && string.IsNullOrEmpty(dto.logo_url))
            {
                dto.logo_url = "";
            }
             ( ImageUrl,  deleteUrl) = await _imageservice.ProcessImageAsync(1000, 19900,10000,imageUrl:dto.logo_url, file: dto.File);
             if (ImageUrl == null && deleteUrl == null)
             {
                 throw new Exception("Failed to process image.");
             }
            
           

           
            var shop = new Domain.Entities.shop(name: dto.Name, d: dto.Description, shopownerid: shownerId.Value, logurl: ImageUrl, deletehashingimage: deleteUrl);
            int newShopId;
            try
            {
              newShopId=  await _Unityofwork.ShopRepository.AddShop(shop);
            } catch (Exception ex)
            {
                _imgur.DeleteImageAsync(deleteUrl).Wait();
                throw new Exception("Failed to add shop to database: " + ex.Message);
            }
            await _Unityofwork.ShopRepository.AddShopCategory(shopid: newShopId, dto.Categories);
            return "Shop opened successfully.";
        }



        public async Task<(List<string> Types, int Page, int PageSize, string Message)> GetShopTypes(int limit = 30, int page = 1, string search = null)
        {
            limit = limit > 50 ? 30 : limit;
            page = page < 1 ? 1 : page;

            // 2️⃣ Query DB
            int offset = (page - 1) * limit;
            var dbResult = await _Unityofwork.ShopRepository
                .GetShopType(offset, limit, search);

            if (dbResult == null)
            {
                throw new Exception("Failed to retrieve shop types from database.");
            }

            return (Types: dbResult, Page: page, PageSize: limit, Message: "Shop types retrieved successfully.");
        }



        public async Task<(List<ShopSumaryDto>,int limit ,int page)> GetShops(int limit = 20, int page = 1, string name = null, string type = null)
        {
            if(limit > 50)
            {
                limit = 20;
            }
            if(page < 1)
            {
                page = 1;
            }
            string cacheKey = $"shops:p={page}:l={limit}:n={name}:t={type}";

            // 1️⃣ Redis = full answer or nothing
            try
            {
                var cached = await _rediscache.GetObjectAsync<List<ShopSumaryDto>>(cacheKey);
                if (cached != null)
                    return (cached,limit,page);
            }
            catch (Exception ex)
            {
               // FileLogger.LogWarning("Redis cache retrieval failed", ex);
            }
            // 2️⃣ DB is source of truth
            int offset = (page - 1) * limit;
            var shops = await _Unityofwork.ShopRepository
                .GetShops(limit, offset, name, type);

            if (shops == null)
                throw new Exception("Failed to retrieve shops from database.");
            var shopDtos = new List<ShopSumaryDto>();
            foreach (var shop in shops)
            {
                var shopDto = new ShopSumaryDto
                {
                    Id = shop.shopid,
                    Name = shop.name,
                    Description = shop.description,
                    url = shop.logoUrl,
                    type = shop.type
                };
                shopDtos.Add(shopDto);
            }


            // 3️⃣ Cache full result
            try
            {
                await _rediscache.SetObjectAsync(
                    cacheKey,
                    shopDtos,
                    TimeSpan.FromMinutes(5)
                );


            }
            catch (Exception ex)
            {
               // FileLogger.LogWarning("Redis cache set failed", ex);
            }
            return (shopDtos,limit,page);
        }



    
    }
    }

  