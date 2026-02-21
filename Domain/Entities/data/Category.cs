using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace onlineshopowner_api.Domain.Entities
{
    public class Category
    {
        public int categoryid;
        public string name;
        

        public Category(int categoryid, string name)
        {
            this.categoryid = categoryid;
            this.name = name;
        }
    }
}