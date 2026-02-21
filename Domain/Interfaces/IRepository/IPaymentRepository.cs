using onlineshopowner_api.Domain.Entities.Payment;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace onlineshopowner_api.Domain.Interfaces.IRepository
{
    public interface IPaymentRepository
    {
        System.Threading.Tasks.Task RegisterPayIn(PayIn payIn);
        System.Threading.Tasks.Task RegisterInternalTransaction(InternalTransaction internalTransaction);
        System.Threading.Tasks.Task RegisterPayOut(PayOut payOut);
    }
}