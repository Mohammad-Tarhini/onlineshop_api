using onlineshopowner_api.Application.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace onlineshopowner_api.Domain.Interfaces.IRepository
{
    public interface IRedisRepository
    {
        Task<(string Status, List<ShopSumaryDto> Dtoshops, string message)> GetShopsByRedis(int limit = 20,
  int pagenb = 1,
  string searchbyshopname = null,
  string searchbyshoptype = null,
  string searchbycategory = null);
        Task<String> SetShopInRedis(List<ShopSumaryDto> shops);
        //Task<(string status, List<ProductRequestDto> productDtos, string message)> GetProductFromRedis(int shopid = 0, int limit = 30, int pagenb = 1, string searchbyproductname = null, string searchbyproductcategory = null, string searchbytype = null);
        //Task<string> SetProductInRedis(ProductRequestDto products);
    }
}
