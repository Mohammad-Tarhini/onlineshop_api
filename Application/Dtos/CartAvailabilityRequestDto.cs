using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace onlineshopowner_api.Application.Dtos
{
    public class CartAvailabilityRequestDto
    {
        public int ShopId { get; set; }
        public List<CartItemRequestDto> CartItemDtos { get; set; }
    }
}