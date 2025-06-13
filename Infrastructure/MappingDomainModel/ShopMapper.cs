using onlineshopowner_api.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace onlineshopowner_api.Infrastructure.MappingDomainModel
{
    public class ShopMapper : IMapper<Domain.Entities.shop, Models.Shop>
    {
        public Models.Shop ToEntity(Domain.Entities.shop shop)
        {
            return new Models.Shop
            { 
                shop_id=shop.shopid,
                name=shop.name,
                logo_url=shop.logoUrl,
                created_date=shop.createddate,
                description=shop.description,
                shopowner_id=shop.shopownerid,
            };
        }
        public Domain.Entities.shop ToDomain(Models.Shop shop)
        {
            if (!shop.shopowner_id.HasValue)
                throw new ArgumentException("Shopowner ID is required");


            return new Domain.Entities.shop( shop.name, shop.description, shop.shopowner_id.Value, shop.logo_url, shop.deletehashimage );
        }
    }
}