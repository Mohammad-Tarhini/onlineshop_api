using onlineshopowner_api.Domain.Constant;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace onlineshopowner_api.Domain.Interfaces.IRepository
{
    public  interface IProductRepository
    {
        Task<ResultCheckdb<int>> GetProductid(string productname, int shopid);
        Task<(bool issuccess, string messege)> addproduct(Domain.Entities.Product product);
        Task<(bool issuccess, string message)> AddUrlImageToProductImages(string imgurl, string deletehash, int productid, bool isprofile);
        Task<ResultCheckdb<Domain.Entities.Product>> GetProductById(int productid);
        Task<ResultCheckdb<List<Domain.Entities.Product>>> GetproducToUser(int shopid = 0, int limit = 30, int offset = 0, string searchbyproductname = null, string searchbycategory = null, string searchbyshoptype = null);
    }
}
