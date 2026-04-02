using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace onlineshopowner_api.Domain.Entities.Payment
{
    public class InternalTransaction
    {
        public int Id { get; set; }
        public int userId { get; set; }
        public int orderId { get; set; }
        public decimal Amount { get; set; }
        public DateTime TransactionDate { get; set; }
        public string description { get; set; }
    }
}