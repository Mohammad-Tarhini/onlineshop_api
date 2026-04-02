using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace onlineshopowner_api.Domain.Entities.Payment
{
    public class PayIn
    {
        public int Id { get; set; }
        public int OrderId { get; set; }

        public decimal Amount { get; set; }

        public string PaymentMethod { get; set; }

        public DateTime PaymentDate { get; set; }
        public string Status { get; set; } // Pending, Paid, Failed,transfered


    }
}