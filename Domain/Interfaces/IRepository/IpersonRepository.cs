
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http.Controllers;
using Twilio.TwiML.Voice;

namespace onlineshopowner_api.Domain.Interfaces.IRepository
{
    public interface IpersonRepository
    {
        //+++++++++++++++new+++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
        Task<int?> GetClientIdByPersonId(int personId);
        Task<int?> GetPersonIdByClientId(int clientId);
        Task<int?> GetShopOwnerIdByPersonId(int personId);
        Task<int?> GetPersonIdByShopOwnerId(int shopOwnerId);
        Task<int?> GetAdminIdByPersonId(int personId);
        Task<int?> GetDeliveryIdByPersonId(int personId);
        Task<Domain.Entities.Person> GetPersonByEmailOrPhonenumber(string email = null, string phoneNumber = null);
        Task<Domain.Entities.Person> GetPersonById(int personId);
        System.Threading.Tasks.Task AddPersonAsync(Domain.Entities.Person person);

        System.Threading.Tasks.Task AddClientByPerson(Domain.Entities.Client client);

        System.Threading.Tasks.Task AddShopOwnerByPerson(Domain.Entities.ShopOwner shopOwner);
        System.Threading.Tasks.Task AddAdminByPerson(Domain.Entities.Admin admin);
        //System.Threading.Tasks.Task AddDeliveryPersonByPersonId(Domain.Entities.del);

        System.Threading.Tasks.Task AddToPandingRegisteration(Domain.Entities.Person person, string otpCode, string role);
        System.Threading.Tasks.Task DeletePendingPerson(string email, string phoneNumber);

        //++++++++++++new end++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
        //T
        // Task<ResultCheckdb<Domain.Entities.Person>> GetPersonByEmailAsync(string email);
        //Task<ResultCheckdb<Domain.Entities.Person>> GetPersonByPhoneNumberAsync(string phoneNumber);
        //Task<ResultCheckdb<Domain.Entities.Person>> GetPersonByCredentialsAsync(string firstName, string lastName);
        //Task<ResultCheckdb<Domain.Entities.Client>> GetClientByPersonAsync(Domain.Entities.Person person);
        //Task<ResultCheckdb<Domain.Entities.ShopOwner>> GetShopOwnerByPersonAsync(Domain.Entities.Person person);
        //Task<UpdateDataProcess> AddPersonAsync(Domain.Entities.Person person);
        //Task<UpdateDataProcess> AssignClientRoleToPersonAsync(Domain.Entities.Person person);

        //Task<UpdateDataProcess> AssignShopOwnerRoleToPersonAsync(Domain.Entities.Person person);
        //Task<ResultCheckdb<Domain.Entities.Person>> GetPersonByPersonId(int personid);
        //Task<ResultCheckdb<Domain.Entities.Admin>> checkAdmainbypersonid(int personid);
        //Task<ResultCheckdb<Domain.Entities.Admin>> GetAdminByPersonAsync(Domain.Entities.Person person);
        //Task<UpdateDataProcess> AssignAdmintRoleToPersonAsync(Domain.Entities.Person person);
        //Task<ResultCheckdb<int>> GetShopOwnerIdByPersonId(int personid);
        //Task<ResultCheckdb<int>> CheckExistDeliveryPersonByPersonId(int personid);
    }
}