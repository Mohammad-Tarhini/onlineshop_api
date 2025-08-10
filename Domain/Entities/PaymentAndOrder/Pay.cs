using onlineshopowner_api.Application.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Web;

namespace onlineshopowner_api.Domain.Entities.PaymentAndOrder
{
    public class Pay
    {
        public List<CartItem> Items { get; set; }
         
        public Decimal latitude {  get; set; }
        public Decimal longitude { get; set; }
        public int ShopId {  get; set; }
        public int DeliveryProviderid { get; set; }
    }
}