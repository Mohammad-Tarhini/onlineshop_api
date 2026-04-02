using onlineshopowner_api.Application.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace onlineshopowner_api.Domain.Interfaces.IRepository
{
    public interface IShopRepository
    {
        Task<int?> GetShopIDByShopownerId(int shopOwnerId);
        Task<int> AddShop(Domain.Entities.shop shop);

        Task<Domain.Entities.shop> GetShopByShopOwnerIdOrShopId(int shopOwnerId = 0, int shopId = 0);
        Task updateShop(Domain.Entities.shop shop);

        Task<List<Domain.Entities.shop>> GetShops(int limit = 20, int offset = 0, string searchbyshopname = null, string searchbyshoptype = null);
        Task<List<string>> GetShopType(int offset, int limit, string search);
        Task AddShopCategory(int shopid, List<int> categoryid);
        Task<(decimal shopLatitude, decimal shopLongitude)?> GetShopLocationById(int shopid);
        //Task<ResultCheckdb<Domain.Entities.shop>> GetShopByShopOwner(Domain.Entities.ShopOwner shopowner);
        //Task<string> createShoponDatabase(Domain.Entities.shop shop);
        //Task<UpdateDataProcess> Updatethelogourl(string urllogo, string deletehash, int shopid);
        //Task<ResultCheckdb<List<string>>> GetShopType(int offset, int limit, string search);
        //Task<ResultCheckdb<List<ShopSumaryDto>>> GetShoptouser(int limit = 20, int offset = 0, string searchbyshopname = null, string searchbyshoptype = null);
        //Task<ResultCheckdb<int>> GetShopByShopOwnerid(int shopownerid);
        //Task<ResultCheckdb<(decimal shoplatitude, decimal shoplan)>> GetShopLocationById(int shopid);
        //Task<ResultCheckdb<(string phonenumber, string email, string shopname)>> GetPhoneNumberAndEmailbyShopid(int shopid);
    }
}
