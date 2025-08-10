using onlineshopowner_api.Application.Dtos;
using onlineshopowner_api.Domain.Constant;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace onlineshopowner_api.Domain.Interfaces.IRepository
{
    public interface IShopRepository
    {
        Task<ResultCheckdb<Domain.Entities.shop>> GetShopByShopOwner(Domain.Entities.ShopOwner shopowner);
        Task<string> createShoponDatabase(Domain.Entities.shop shop);
        Task<UpdateDataProcess> Updatethelogourl(string urllogo, string deletehash, int shopid);
        Task<ResultCheckdb<List<string>>> GetShopType(int offset, int limit, string search);
        Task<ResultCheckdb<List<ShopSumaryDto>>> GetShoptouser(int limit = 20, int offset = 0, string searchbyshopname = null, string searchbyshoptype = null);
        Task<ResultCheckdb<int>> GetShopByShopOwnerid(int shopownerid);
        Task<ResultCheckdb<(decimal shoplatitude, decimal shoplan)>> GetShopLocationById(int shopid);
        Task<ResultCheckdb<(string phonenumber, string email, string shopname)>> GetPhoneNumberAndEmailbyShopid(int shopid);
    }
}
