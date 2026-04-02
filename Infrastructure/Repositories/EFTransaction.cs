using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.Entity;

namespace onlineshopowner_api.Infrastructure.Repositories
{
    
    using onlineshopowner_api.Application.Interfaces.Iservices;
    using System.Data.Entity;

    public class EfTransaction : IDBTransaction
    {
        private readonly DbContextTransaction _transaction;

        public EfTransaction(DbContextTransaction transaction)
        {
            _transaction = transaction;
        }

        public void Commit()
        {
            _transaction.Commit();
        }

        public void Rollback()
        {
            _transaction.Rollback();
        }

        public void Dispose()
        {
            _transaction.Dispose();
        }
    }

}