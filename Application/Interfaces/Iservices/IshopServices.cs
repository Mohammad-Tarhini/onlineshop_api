using onlineshopowner_api.Application.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace onlineshopowner_api.Application.Interfaces.Iservices
{
    public interface IshopServices
    {
        Task<string> OpenShop(OpenNewShopDto dto);
        Task<string> updataShop(OpenNewShopDto dto);
        Task<(List<ShopSumaryDto> ,int limit ,int page)> GetShops(int limit = 20, int page = 1, string name = null, string type = null);
        Task<(List<string> Types, int Page, int PageSize, string Message)> GetShopTypes(int limit = 30, int page = 1, string search = null);
        //Task<(bool IsSuccess, string message)> PutProfileForShop(UpdatProfileShopeDto dto);
        //Task<(bool IsSuccess, string message)> OpenShop(OpenNewShopDto dto);
        //Task<(bool IsSuccess, List<string> Types, int Page, int PageSize, string Message)>GetShopTypes(int limit = 30, int page = 1, string search = null);
        //Task<(bool IsSuccess, List<ShopSumaryDto> Shops, string Message)> GetShops(int limit = 20, int page = 1, string name = null, string type = null);
    }
}
