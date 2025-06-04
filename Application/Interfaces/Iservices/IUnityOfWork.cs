using onlineshopowner_api.Domain.Interfaces.IRepository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace onlineshopowner_api.Application.Interfaces.Iservices
{
    public interface IUnityOfWork:IDisposable
    {
        Task<int> CommitAsync();
        IpersonRepository PersonRepository { get; set; }

    }
}