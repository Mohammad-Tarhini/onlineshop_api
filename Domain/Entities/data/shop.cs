using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace onlineshopowner_api.Domain.Entities
{
    public class shop
    {
        public int shopid { get; set; }
        public string name { get; set; }
        public string description { get; set; }
        public string logoUrl { get; set; }

        public DateTime createddate { get; set; }

        public int shopownerid {  get; set; }

        public string deletehashingimage {  get; set; }

        public string type { get; set; } 

        public decimal shoplatitude { get; set; }

        public decimal shopLongitude { get; set; }

        public shop(string name,string d ,int shopownerid, string logurl = null,string deletehashingimage=null)
        {
            
            this.name = name;
            this.description = d;
            this.logoUrl = logurl;
            this.shopownerid = shopownerid;
            this.deletehashingimage = deletehashingimage;
        }
        public shop() { }



    }
}