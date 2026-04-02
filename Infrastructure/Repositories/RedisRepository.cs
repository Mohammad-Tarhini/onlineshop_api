using onlineshopowner_api.Application.Dtos;
using onlineshopowner_api.Domain.Entities;
using onlineshopowner_api.Domain.Interfaces.IExternalServices;
using onlineshopowner_api.Domain.Interfaces.IRepository;
using onlineshopowner_api.Infrastructure.Models;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.UI;
using Unity.Interception.Interceptors.TypeInterceptors.VirtualMethodInterception.InterceptingClassGeneration;

namespace onlineshopowner_api.Infrastructure.Repositories
{
    public class RedisRepository : IRedisRepository
    {
        private readonly IRedisCacheService _rediscache;
        public static int ProductRedisCount = 0;
        public RedisRepository(IRedisCacheService rediscache)
        {
            _rediscache = rediscache;
        }
        public async Task<(string Status, List<ShopSumaryDto> Dtoshops, string message)> GetShopsByRedis(int limit = 20,
   int pagenb = 1,
   string searchbyshopname = null,
   string searchbyshoptype = null,
   string searchbycategory = null)
        {
            try
            {
                int offset = (pagenb - 1) * limit;

                // Step 1: Get all keys (or a large range if you expect many shops)
                var allKeys = await _rediscache.GetSortedSetRangeByRankAsync("shops:index", 0, -1);

                var filteredShops = new List<ShopSumaryDto>();
                int countkey = 0;
                // Step 2: Load shops and apply filtering
                foreach (var redisKey in allKeys)
                {
                    if (countkey < offset)
                    {
                        countkey++;
                        continue;
                    }
                    if (countkey > limit + offset)
                        break;
                    var shop = await _rediscache.GetObjectAsync<ShopSumaryDto>(redisKey);

                    if (shop == null)
                    {
                        countkey++;
                        continue;
                    }

                    // Apply filters
                    if (!string.IsNullOrEmpty(searchbyshopname) &&
                        !shop.Name.Contains(searchbyshopname))
                    {
                        countkey++;
                        continue;
                    }

                    if (!string.IsNullOrEmpty(searchbyshoptype) &&
                        !shop.type.Equals(searchbyshoptype, StringComparison.OrdinalIgnoreCase))
                    {
                        countkey++;
                        continue;
                    }
                    filteredShops.Add(new ShopSumaryDto
                    {
                        Id = shop.Id,
                        Name = shop.Name,
                        Description = shop.Description,
                        url = shop.url
                    });
                    countkey++;
                }
                if (filteredShops.Count > 0 && filteredShops.Count == limit)
                    return ("fullsuccess", filteredShops, "good");
                if (filteredShops.Count > 0 && filteredShops.Count < limit)
                {
                    int c = allKeys.Count();
                    return ("halfsuccess", filteredShops, c.ToString());
                }
                if (filteredShops.Count == 0)
                    return ("nodata", null, "fetchalldatafromdatabase");
                else
                    return ("error", null, "nono");
            }
            catch (Exception ex)
            {
                {
                    return ("error", null, ex.Message);
                }
            }
        }
        public async Task<String> SetShopInRedis(List<ShopSumaryDto> shops)
        {
            const string redisSortedSetKey = "shops:index";

            long redisCount = await _rediscache.GetSortedSetLengthAsync(redisSortedSetKey);
            int ShopToAdd = 50 - (int)redisCount;
            if (ShopToAdd > 0)
            {
                int i = 0;
                foreach (ShopSumaryDto shop in shops)
                {
                    if (i >= ShopToAdd) break;
                    string redisShopKey = $"shops:shopid:{shop.Id}";
                    await _rediscache.SetObjectAsync(redisShopKey, shop);
                    await _rediscache.AddToSortedSetAsync(redisSortedSetKey, redisShopKey, shop.Id);
                    ShopToAdd++;
                }
                return "ok";
            }
            return "no";

        }
        //public async Task<string> SetProductInRedis(ProductRequestDto product)
        //{
        //    try
        //    {
        //        const string redisSortedSetKey = "product:index";

        //        string redisProductKey = $"shops:shopid:{product.Id}";
        //        await _rediscache.SetObjectAsync(redisProductKey, product);
        //        await _rediscache.AddToSortedSetAsync(redisSortedSetKey, redisProductKey, product.Id);
        //        return "ok";
        //    } catch (Exception ex)
        //    {
        //        return "No";
        //    }



        //}


        //public async Task<(string status,List<ProductRequestDto> productDtos,string message)> GetProductFromRedis(int shopid = 0, int limit = 30, int pagenb = 1, string searchbyproductname = null,  string searchbyproductcategory = null,string searchbytype=null)
        //{
        //    try
        //    {

        //        int offset = (pagenb - 1) * limit;

        //        // Step 1: Get all keys (or a large range if you expect many shops)
        //        var allKeys = await _rediscache.GetSortedSetRangeByRankAsync("products:index", 0, -1);

        //        var filteredShops = new List<ProductRequestDto>();
        //        int countkey = 0;
        //        // Step 2: Load shops and apply filtering
        //        foreach (var redisKey in allKeys)
        //        {
        //            if (countkey < offset)
        //                countkey++;
        //            continue;
        //            if (countkey > limit + offset)
        //                break;
        //            var product = await _rediscache.GetObjectAsync<ProductRequestDto>(redisKey);

        //            if (product == null)
        //            {
        //                countkey++;
        //                continue;
        //            }
        //            if(shopid != 0)
        //            {
        //                if(product.shop_id != shopid)
        //                {
        //                    countkey++;
        //                    continue;
        //                }
        //            }

        //            // Apply filters
        //            if (!string.IsNullOrEmpty(searchbyproductname) &&
        //                !product.name.Contains(searchbyproductname))
        //            {
        //                countkey++;
        //                continue;
        //            }

        //            if (!string.IsNullOrEmpty(searchbyproductcategory)&&!product.category.Contains(searchbyproductcategory))
        //            {
        //                countkey++;
        //                continue;
        //            }
        //            if (!string.IsNullOrEmpty(searchbytype) && product.shop_type.Contains(searchbytype))
        //            {
        //                countkey++;
        //                continue;
        //            }



        //            filteredShops.Add(product);
        //            countkey++;
        //        }
        //        if (filteredShops.Count > 0 && filteredShops.Count == limit)
        //            return ("fullsuccess", filteredShops, "good");
        //        if (filteredShops.Count > 0 && filteredShops.Count < limit)
        //        {
        //            int c = allKeys.Count();
        //            return ("halfsuccess", filteredShops, c.ToString());
        //        }
        //        if (filteredShops.Count == 0)
        //            return ("nodata", null, "fetchalldatafromdatabase");
        //        else
        //            return ("error", null, "nono");
        //    }
        //    catch (Exception ex)
        //    {
        //        {
        //            return ("error", null, ex.Message);
        //        }
        //    }

        //}
    

    }
}
