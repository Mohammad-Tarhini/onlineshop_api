//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace onlineshopowner_api.Infrastructure.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class Payment
    {
        public int payment_id { get; set; }
        public Nullable<int> order_id { get; set; }
        public string payment_status { get; set; }
        public string payment_method { get; set; }
        public Nullable<decimal> tax_amount { get; set; }
    
        public virtual clientorder clientorder { get; set; }
    }
}
