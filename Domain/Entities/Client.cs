using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace onlineshopowner_api.Domain.Entities
{
    public class Client
    {
        public int ClientId { get; set; }
        public int PersonId { get; set; }

        public Client(int ClientId, int PersonId)
        {
            this.ClientId = ClientId;
            this.PersonId = PersonId;
        }

    }
}