using onlineshopowner_api.Domain.Entities;
using onlineshopowner_api.Domain.Interfaces;
using onlineshopowner_api.Infrastructure.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace onlineshopowner_api.Infrastructure.MappingDomainModel
{
    public class AdminMapper : IMapper<Domain.Entities.Admin, Models.admain>
    {
        public Domain.Entities.Admin ToDomain(Models.admain admin)
        {
           

            return new Domain.Entities.Admin(admin.admin_id, admin.person_id ?? throw new ArgumentNullException(nameof(admin.person_id)));
        }
        public Models.admain ToEntity(Domain.Entities.Admin admin)
        {
            return new Models.admain
            {
                admin_id = admin.admainId,
                person_id = admin.personid
            };
        }
    }
}