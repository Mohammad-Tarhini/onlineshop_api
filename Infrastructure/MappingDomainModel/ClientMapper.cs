using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using onlineshopowner_api.Domain.Interfaces;

namespace onlineshopowner_api.Infrastructure.MappingDomainModel
{
    public class ClientMapper: IMapper<Domain.Entities.Client, Models.Client>
    {
        public Domain.Entities.Client  ToDomain(Models.Client client)
        {
            return new Domain.Entities.Client(client.client_id, client.person_id ?? throw new ArgumentNullException(nameof(client.person_id)));
        }
        public Models.Client ToEntity(Domain.Entities.Client client) {

            return new Models.Client
            {
                client_id = client.ClientId,
                person_id = client.PersonId,

            };
        
        }
    }
}