using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace onlineshopowner_api.Application.Dtos.PaymentAndOrder
{
    public class OrderItemRequestDto
    {
        [Required]
        public int ProductId { get; set; }


        public string ProductName { get; set; }

        public int Quantity { get; set; }

        public string description { get; set; }
    }
}