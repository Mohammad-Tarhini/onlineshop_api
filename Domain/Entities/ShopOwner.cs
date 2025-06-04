using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace onlineshopowner_api.Domain.Entities
{
    public class ShopOwner
    {
        public int ShopOwnerId { get; set; }
        public int PersonId { get; set; }

        public ShopOwner(int shopownerid, int personid)
        {
            ShopOwnerId = shopownerid;
            PersonId = personid;
        }
    }
}