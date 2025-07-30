using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace onlineshopowner_api.Domain.Entities
{
    public class Product
    {
        public string name { get; set; }
       
        public string description { get; set; }
      
        public decimal price { get; set; }

       public int imgurid {  get; set; }
        public int category_id { get; set; }

        public string status { get; set; }

        public int quentity { get; set; }
   
        public int shop_id { get; set; }

        public int product_id { get; set; }
    }
}