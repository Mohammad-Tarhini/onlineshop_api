using onlineshopowner_api.Application.Dtos;
using onlineshopowner_api.Application.Interfaces.Iservices;
using onlineshopowner_api.Application.Interfaces.Ivalidator;
using onlineshopowner_api.Application.Validatorandclean;
using onlineshopowner_api.Domain.Constant;
using onlineshopowner_api.Domain.Entities;
using onlineshopowner_api.Domain.Interfaces.IExternalServices;
using onlineshopowner_api.Domain.Interfaces.IRepository;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using System.Xml.Linq;

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
        

        public ShopServices(IUserContextService usercontext, IUnityOfWork unityofwork, IImageService imageservice, IImgur imgur,IRedisCacheService redisCacheService,IRedisRepository redisRepository)
        {
            _usercontext = usercontext;
            _Unityofwork = unityofwork;
            _imageservice = imageservice;
            _rediscache = redisCacheService;
            _imgur = imgur;
            _redisRepository = redisRepository;
        }
        public async Task<(bool IsSuccess, string message)> PutProfileForShop(UpdatProfileShopeDto dto)
        {
            // Get user ID from token
            try
            {
                _userId = _usercontext.GetUserId();
            }
            catch (UnauthorizedAccessException ex)
            {
                return (false, ex.Message);
            }

            // Check if image is provided
            if (dto.logo_url == null && dto.File == null)
                return (false, "No image provided.");

            // Check if this user is a valid shop owner
            var personResult = await _Unityofwork.PersonRepository.GetPersonByPersonId(_userId);
            if (!personResult.IsSuccess || !personResult.IsFound)
                return (false, personResult.Error);

            var shopOwnerResult = await _Unityofwork.PersonRepository.GetShopOwnerByPersonAsync(personResult.Value);
            if (!shopOwnerResult.IsSuccess || !shopOwnerResult.IsFound)
                return (false, shopOwnerResult.Error);

            var shopOwner = shopOwnerResult.Value;

            // Check if the shop belongs to the shop owner
            var shopResult = await _Unityofwork.ShopRepository.GetShopByShopOwner(shopOwner);
            if (!shopResult.IsSuccess || !shopResult.IsFound)
                return (false, shopResult.Error);

            var shopEntity = shopResult.Value;

            if (shopEntity.shopid != dto.shopid)
                return (false, "You are not the owner of this shop.");

            // Delete old image if exists
            if (!string.IsNullOrWhiteSpace(shopEntity.logoUrl))
            {
                var (deleteSuccess, message) = await _imgur.DeleteImageAsync(shopEntity.deletehashingimage);
                if (!deleteSuccess)
                    return (false, message);
            }

            // Upload new image to cloud
            string logoUrl = null;
            string deleteHash = null;

            try
            {
                bool uploadSuccess;
                string cloudResponse;

                if (dto.logo_url != null)
                {
                    (uploadSuccess, cloudResponse, deleteHash) = await _imageservice.ProcessImageAsync(100, 199, 100, imageUrl: dto.logo_url);
                }
                else
                {
                    (uploadSuccess, cloudResponse, deleteHash) = await _imageservice.ProcessImageAsync(10000, 19900, 109990, file: dto.File);
                }

                if (!uploadSuccess)
                    return (false, cloudResponse);

                logoUrl = cloudResponse;
            }
            catch
            {
                return (false, "Failed to process image.");
            }



            // Update shop logo URL in DB
            try
            {
                var updateResult = await _Unityofwork.ShopRepository.Updatethelogourl(logoUrl, deleteHash, dto.shopid);
                if (updateResult == UpdateDataProcess.Success)
                    return (true, "Shop profile image updated successfully.");
                else
                    return (false, "Database update failed.");
            }
            catch (Exception ex)
            {
                return (false, $"Database update error: {ex.Message}");
            }
        }
        public async Task<(bool IsSuccess, string message)> OpenShop(OpenNewShopDto dto)
        {
            try
            {
                _userId = _usercontext.GetUserId();
                _role = _usercontext.GetUserRole();
                if (_role != "shopowner") return (false, "the role is wrong ");
            }
            catch (UnauthorizedAccessException ex)
            {
                return (false, ex.Message);
            }
            //check if userid is shopowner
            Domain.Entities.ShopOwner shopowner;
            try
            {
                var personResultCheckdb = await _Unityofwork.PersonRepository.GetPersonByPersonId(_userId);

                if (!personResultCheckdb.IsSuccess) return (false, personResultCheckdb.Error);
                if (!personResultCheckdb.IsFound) return (false, personResultCheckdb.Error);
                try
                {
                    var shopownerResultCheckdb = await _Unityofwork.PersonRepository.GetShopOwnerByPersonAsync(personResultCheckdb.Value);
                    if (!shopownerResultCheckdb.IsSuccess) return (false, shopownerResultCheckdb.Error);
                    if (!shopownerResultCheckdb.IsFound) return (false, shopownerResultCheckdb.Error);
                    shopowner = shopownerResultCheckdb.Value;

                }
                catch (Exception ex) { return (false, ex.Message); }
            }
            catch (Exception ex) { return (false, ex.Message); }

            //check if this shopowner have shop before and add directly
            var shop = await _Unityofwork.ShopRepository.GetShopByShopOwner(shopowner);
            if (!shop.IsSuccess) return (false, shop.Error);
            if (shop.IsFound) return (false, shop.Error);
            Clean.CleanStrings(dto);




            //add the shop
            try
            {
                var theshop = new Domain.Entities.shop(name: dto.Name, d: dto.Description, shopownerid: shopowner.ShopOwnerId);
                var addshop = await _Unityofwork.ShopRepository.createShoponDatabase(theshop);
                if (addshop == "Success")
                {
                    return (true, "congrats the new shop");
                }
                else { return (false, addshop); }

            }
            catch (Exception ex)
            {
                return (false, ex.Message);

            }
        }
        public async Task<(bool issuccess,List<string> types, int page,int pagesize, string message)> GetShopTypes(int limit = 30, int page = 1,string search=null)
        {
            try { 
            if (limit > 50) { limit = 30; }
            

            const string redisKey = "shop_types:cached_list";
            var cachedlist = await _rediscache.GetObjectAsync<List<string>>(redisKey);
                if (cachedlist==null)
                {
                    cachedlist=new List<string>();
                }
                int offset = (page - 1) * limit;
                if (!string.IsNullOrWhiteSpace(search))
                {
                    // Filter Redis list by search
                    search=search.ToLower();
                  cachedlist = cachedlist.Where(t => t.Contains(search)).ToList();
                }
                
                if (cachedlist.Count > offset+limit)
                { 
                    var result=cachedlist.Skip(offset).Take(limit).ToList();
                    return (true, result, page, limit, "from Redis");
                
                }
                var resultpartial=cachedlist.Skip(offset).Take(limit).ToList();
                int missingcount=limit-resultpartial.Count;

                int dbOffset = cachedlist.Count;

                var dbResult = await _Unityofwork.ShopRepository.GetShopType(dbOffset, missingcount,search);
                if ( !dbResult.IsSuccess)
                {
                    return (false, null,0,0, dbResult.Error);
                }
                //cachedlist.AddRange(dbResult.Value);
                // var addtoredis = cachedlist.GetRange(0, 40);
                //var addtoredis=cachedlist.Take(40).ToList();
                int remainingSpace = 40 - cachedlist.Count;
                if (remainingSpace > 0)
                {
                   
                    await _rediscache.SetObjectAsync(redisKey, dbResult.Value.Take(remainingSpace), TimeSpan.FromHours(2));
                }
                await _rediscache.SetObjectAsync(redisKey, cachedlist, TimeSpan.FromHours(2));
                var final=resultpartial.Concat(dbResult.Value).ToList();
                if (final.Count == 0)
                    return (false, null, page, limit, "No data on this page");
                return (true, final,page,limit,null);
            }catch(Exception ex)
            {
                return(false,null,0,0,ex.Message+"dd");
            }
            
        }
        public async Task<(bool issuccess,List<ShopSumaryDto> Dtoshops , string message)> GetShops( int limit = 20,  int pagenb = 1,  string searchbyshopname = null,  string searchbyshoptype = null)
        {
            try
            {
                int allkeyscount;
                var (status, redisshopdto, message) = await _redisRepository.GetShopsByRedis(limit, pagenb, searchbyshopname, searchbyshoptype);
                int offset;
                if (status== "fullsuccess")
                return (true,redisshopdto,null);
                else 
                {
                    var isint=int.TryParse(message,out  allkeyscount);
                    if (isint)
                     offset = allkeyscount+(pagenb - 1) * limit;
                    else offset = (pagenb - 1) * limit;
                }
               
                var resultdb=await _Unityofwork.ShopRepository.GetShoptouser(limit,offset,searchbyshopname,searchbyshoptype);
                if (resultdb.IsSuccess) {
                    if (allkeyscount < 50)
                    {
                      await  _redisRepository.SetShopInRedis(resultdb.Value);
                    }
                    return (true, resultdb.Value, ""); }
                else  
                { return (false, null, resultdb.Error); }
            }
            catch (Exception ex) 
            {
                return(false, null,ex.Message);
            }
            




           
        }



    }
}
  