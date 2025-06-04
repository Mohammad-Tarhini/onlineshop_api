using onlineshopowner_api.Domain.Constant;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace onlineshopowner_api.Domain.Interfaces.IRepository
{
    public interface IpersonRepository
    {
         Task<ResultCheckdb<Domain.Entities.Person>> GetPersonByEmailAsync(string email);
        Task<ResultCheckdb<Domain.Entities.Person>> GetPersonByPhoneNumberAsync(string phoneNumber);
        Task<ResultCheckdb<Domain.Entities.Person>> GetPersonByCredentialsAsync(string firstName, string lastName);
        Task<ResultCheckdb<Domain.Entities.Client>> GetClientByPersonAsync(Domain.Entities.Person person);
        Task<ResultCheckdb<Domain.Entities.ShopOwner>> GetShopOwnerByPersonAsync(Domain.Entities.Person person);
        Task<UpdateDataProcess> AddPersonAsync(Domain.Entities.Person person);
        Task<UpdateDataProcess> AssignClientRoleToPersonAsync(Domain.Entities.Person person);

        Task<UpdateDataProcess> AssignShopOwnerRoleToPersonAsync(Domain.Entities.Person person);
    }
}