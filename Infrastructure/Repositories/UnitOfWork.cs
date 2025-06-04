using onlineshopowner_api.Application.Interfaces.Iservices;
using onlineshopowner_api.Domain.Interfaces.IRepository;
using onlineshopowner_api.Infrastructure.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace onlineshopowner_api.Infrastructure.Repositories
{
    public class UnitOfWork: IUnityOfWork
    {
        private readonly online_shopEntities _db;
        public IpersonRepository PersonRepository { get;  set; }
        public UnitOfWork(online_shopEntities dbContext, IpersonRepository personRepository)
        {
            _db = dbContext;
            PersonRepository = personRepository;
        }
        public async Task<int> CommitAsync()
        {
            return await _db.SaveChangesAsync();
        }
        public void Dispose()
        {
            _db.Dispose();
        }

    }
}
