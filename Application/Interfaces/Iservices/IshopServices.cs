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
        Task<(bool IsSuccess, string message)> PutProfileForShop(UpdatProfileShopeDto dto);
        Task<(bool IsSuccess, string message)> OpenShop(OpenNewShopDto dto);
        Task<(bool issuccess, List<string> types, int page, int pagesize, string message)> GetShopTypes(int limit = 30, int page = 1, string search = null);
        Task<(bool issuccess, List<ShopSumaryDto> Dtoshops, string message)> GetShops(int limit = 20, int pagenb = 1, string searchbyshopname = null, string searchbyshoptype = null);
    }
}
