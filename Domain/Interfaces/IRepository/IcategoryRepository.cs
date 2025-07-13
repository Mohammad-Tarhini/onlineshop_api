using onlineshopowner_api.Domain.Constant;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace onlineshopowner_api.Domain.Interfaces.IRepository
{
    public interface IcategoryRepository
    {
        Task<ResultCheckdb<Domain.Entities.Category>> checkIfCategoryExist(string categoryname);
         Task<string> Addcategory(string categoryname);
    }
}
