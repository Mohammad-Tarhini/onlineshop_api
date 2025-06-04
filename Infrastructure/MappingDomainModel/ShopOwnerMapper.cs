using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using onlineshopowner_api.Domain.Interfaces;


namespace onlineshopowner_api.Infrastructure.MappingDomainModel
{
    public class ShopOwnerMapper:IMapper<Domain.Entities.ShopOwner,Models.ShopOwner>
    {
        public Domain.Entities.ShopOwner ToDomain(Models.ShopOwner shopowner)
        {
            return new Domain.Entities.ShopOwner(shopowner.shopowner_id, shopowner.person_id ?? throw new ArgumentNullException(nameof(shopowner.person_id)));
        }
        public Models.ShopOwner ToEntity(Domain.Entities.ShopOwner shopowner)
        {
            return new Models.ShopOwner
            {
                shopowner_id = shopowner.ShopOwnerId,
                person_id = shopowner.PersonId
            };
        }
    }
}