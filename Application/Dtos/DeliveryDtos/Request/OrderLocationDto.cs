using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace onlineshopowner_api.Application.Dtos
{
    public class OrderLocationDto
    {
        public decimal latitude {  get; set; }
        public decimal longitude { get; set; }

        public int shopid {  get; set; }

    }
}