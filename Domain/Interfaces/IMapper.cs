using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace onlineshopowner_api.Domain.Interfaces
{
    public  interface IMapper<TDomain,TEntity>
    {
        TEntity ToEntity(TDomain d);
        TDomain ToDomain(TEntity e);
    }
}
