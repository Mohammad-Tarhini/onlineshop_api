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
        Task<UpdateDataProcess> createShoponDatabase(Domain.Entities.shop shop);
        Task<UpdateDataProcess> Updatethelogourl(string urllogo, string deletehash, int shopid);
    }
}
