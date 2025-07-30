using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace onlineshopowner_api.Application.Dtos
{
    public class CartItemCheckResponseDto
    {
        public int ProductId { get; set; }
        public string ProductName { get; set; }
        public bool IsAvailable { get; set; }
        public int? AvailableQuantity { get; set; }
        public decimal? CurrentPrice { get; set; }
        public string Reason { get; set; }
    }
}