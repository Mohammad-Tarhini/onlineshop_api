using onlineshopowner_api.Infrastructure.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using onlineshopowner_api.Domain.Interfaces;
using System.Data.Entity;
using onlineshopowner_api.Domain.Interfaces.IRepository;
using System.Data.Entity.Infrastructure;
using onlineshopowner_api.Domain.Entities;
using onlineshopowner_api.Application.Dtos;
using StackExchange.Redis;

namespace onlineshopowner_api.Infrastructure.Repositories
{
    public class PersonRepository:IpersonRepository
    {
        private readonly online_shopEntities2 _dbContext;
       
        public PersonRepository(online_shopEntities2 dbContext)
        {
            _dbContext = dbContext;
          
          
        }

        //++++++++++++++++++news ++++++++++++++++++++++++++++++++++++++++++++++++++++++
        public async Task<int?> GetClientIdByPersonId(int personId)
        {
            var dbClient = await _dbContext.Clients.FirstOrDefaultAsync(c => c.person_id == personId);
            return dbClient?.client_id;

        }
        public async Task<int?> GetPersonIdByClientId(int clientId)
        {
            var dbClient = await _dbContext.Clients.FirstOrDefaultAsync(c => c.client_id == clientId);
            return dbClient?.person_id;
        }
        public async Task<int?> GetShopOwnerIdByPersonId(int personId)
        {
            var dbShopOwner = await _dbContext.ShopOwners.FirstOrDefaultAsync(so => so.person_id == personId);
            return dbShopOwner?.shopowner_id;
        }
        public async Task<int?>GetPersonIdByShopOwnerId(int shopOwnerId)
        {
            var dbShopOwner = await _dbContext.ShopOwners.FirstOrDefaultAsync(so => so.shopowner_id == shopOwnerId);
            return dbShopOwner?.person_id;
        }
        //public async Task<int?> GetPersonIdByClientId(int clientId)
        //{
        //    var dbClient = await _dbContext.de.FirstOrDefaultAsync(c => c.client_id == clientId);
        //    return dbClient?.person_id;
        //}
        public async Task<int?> GetAdminIdByPersonId(int personId)
        {
            var dbAdmin = await _dbContext.admains.FirstOrDefaultAsync(a => a.person_id == personId);
            return dbAdmin?.admin_id;
        }
        public async Task<int?> GetDeliveryIdByPersonId(int personId)
        {
            var dbDeliveryPerson = await _dbContext.DeliveryProviders.FirstOrDefaultAsync(dp => dp.person_id == personId);
            return dbDeliveryPerson?.delivery_Id;
        }
        public async Task<Domain.Entities.Person> GetPersonByEmail(string email)
        {
            var query=_dbContext.People.AsQueryable();
            query=query.Where(p=>p.email== email);
            var dbPerson= await query.FirstOrDefaultAsync();
            if (dbPerson != null)
            {
                return null;
            }
            return new Domain.Entities.Person
            {
                Id=dbPerson.person_id,
                Email=dbPerson.email,
                FirstName=dbPerson.first_name,
                LastName=dbPerson.last_name,
                PhoneNumber=dbPerson.phone_number,
                CreatedDate=dbPerson.created_date??DateTime.UtcNow,
            };
        }
        public async Task<Domain.Entities.Person> GetPersonByEmailOrPhonenumber(string email = null, string phoneNumber = null)
        {
            try
            {
                var query = _dbContext.People.AsQueryable();

                if (!string.IsNullOrEmpty(email))
                {
                    query = query.Where(p => p.email == email);
                }

                if (!string.IsNullOrEmpty(phoneNumber))
                {
                    query = query.Where(p => p.phone_number == phoneNumber);
                }

                var dbPerson = await query.FirstOrDefaultAsync();

                if (dbPerson == null)
                    return null;

                return new Domain.Entities.Person
                {
                    Id = dbPerson.person_id,
                    Email = dbPerson.email,
                    FirstName = dbPerson.first_name,
                    LastName = dbPerson.last_name,
                    PhoneNumber = dbPerson.phone_number,
                    CreatedDate = dbPerson.created_date ?? DateTime.UtcNow,
                    Password=dbPerson.password,
                };
            }
            catch (Exception ex)
            {
                throw new Exception(ex.ToString());
            }
        }
        public async Task<Domain.Entities.Person> GetPersonById(int personId)
        {
            var dbPerson = await _dbContext.People
                .FirstOrDefaultAsync(p => p.person_id == personId);
            return new Domain.Entities.Person
            {
                Id = dbPerson.person_id,
                Email = dbPerson.email,
                FirstName = dbPerson.first_name,
                LastName = dbPerson.last_name,
                PhoneNumber = dbPerson.phone_number,
                CreatedDate = dbPerson.created_date.Value
            };
        }

        public  Task AddPersonAsync(Domain.Entities.Person person)
        {
            var personEF = new Models.Person
            {
                person_id = person.Id,
                email = person.Email,
                first_name = person.FirstName,
                last_name = person.LastName,
                phone_number = person.PhoneNumber,
                created_date = person.CreatedDate,
                password = person.Password
            };

             _dbContext.People.Add(personEF);

            return Task.CompletedTask;
        }
        public  Task AddClientByPerson( Domain.Entities.Client client)
        {
            var dbClient = new Models.Client
            {
                person_id = client.PersonId,
            };
            _dbContext.Clients.Add(dbClient);
            return Task.CompletedTask;
        }
        public  Task AddShopOwnerByPerson(Domain.Entities.ShopOwner shopOwner)
        {
            var dbShopOwner = new Models.ShopOwner
            {
                person_id = shopOwner.PersonId,
            };
            _dbContext.ShopOwners.Add(dbShopOwner);
            return Task.CompletedTask;
        }
        public  Task AddAdminByPerson(Domain.Entities.data.Admain admin)
        {
            var dbAdmin = new Models.admain
            {
                person_id = admin.PersonId,
            };
            _dbContext.admains.Add(dbAdmin);
            return Task.CompletedTask;
        }
        //public async Task AddDeliveryPersonByPerson(int personId)
        //{
        //    var dbDeliveryPerson = new Models.PersonDelivery
        //    {
        //        person_id = personId,
        //    };
        //    _dbContext.PersonDeliveries.Add(dbDeliveryPerson);
        //}
        public  Task AddToPandingRegisteration(Domain.Entities.Person person, string otpCode, string role)
        {
            var dbPendingRegistation = new PendingRegistration
            {
                FirstName=person.FirstName,
                LastName=person.LastName,
                Email=person.Email,
                PhoneNumber=person.PhoneNumber,
                Sex=person.Sex,
                OtpCode=otpCode,
                PasswordHash=person.Password,
               // OtpExpiresAt=DateTime.,
                CreatedAt=DateTime.Now,
                Role=role
                
            };
            _dbContext.PendingRegistrations.Add(dbPendingRegistation);
            return Task.CompletedTask;
        }
        public async Task DeletePendingPerson(string email , string phoneNumber)
        {
            _dbContext.PendingRegistrations.Remove(_dbContext.PendingRegistrations.FirstOrDefault(pr => pr.Email == email || pr.PhoneNumber == phoneNumber));
            await _dbContext.SaveChangesAsync();
            
        }






        //++++++++++++++++++end news ++++++++++++++++++++++++++++++++++++++++++++++++++++++


        //public async Task<ResultCheckdb<int>>CheckExistDeliveryPersonByPersonId(int personid)
        //{
        //    try
        //    {
        //        if (personid < 0)
        //        {
        //            return new ResultCheckdb<int>
        //            {
        //                IsSuccess = false
        //            };

        //        }
        //        int Deliveryid = _dbContext.PersonDeliveries.Where(s => s.person_id == personid).Select(s => s.delivery_person_id).FirstOrDefault();
        //        if (Deliveryid == 0)
        //        {
        //            return new ResultCheckdb<int>
        //            {
        //                IsSuccess = true,
        //                IsFound = false
        //            };

        //        }
        //        else
        //        {
        //            return new ResultCheckdb<int>
        //            {
        //                IsSuccess = true,
        //                IsFound = true
        //            };
        //        }
        //    }
        //    catch (Exception ex) 
        //    {
        //        return new ResultCheckdb<int>
        //        {
        //            IsSuccess = false
        //        };
        //    }

        //}



        //public async Task<Domain.Entities.Person> GetPersonByEmailAsync(string email)
        //{
        //        var dbPerson = await _dbContext.People
        //            .FirstOrDefaultAsync(p => p.email == email);
        //    return new Domain.Entities.Person
        //    {
        //        Email = dbPerson.email,
        //        FirstName= dbPerson.first_name,
        //        LastName= dbPerson.last_name,
        //        PersonId= dbPerson.person_id,
        //        PhoneNumber= dbPerson.phone_number,
        //        CreatedDate= dbPerson.created_date.Value,


        //    };

        //}

        //public async Task<Domain.Entities.Person>GetPersonByPhoneNumberAsync(string phoneNumber)
        //{

        //        var dbPerson = await _dbContext.People
        //            .FirstOrDefaultAsync(p => p.phone_number == phoneNumber);
        //        return new Domain.Entities.Person
        //        {
        //            Email = dbPerson.email,
        //            FirstName = dbPerson.first_name,
        //            LastName = dbPerson.last_name,
        //            PersonId = dbPerson.person_id,
        //            PhoneNumber = dbPerson.phone_number,
        //            CreatedDate = dbPerson.created_date.Value

        //        };   

        //}

        //public async Task<ResultCheckdb<Domain.Entities.Person>> GetPersonByCredentialsAsync(string firstName, string lastName) 
        //{if (firstName == null || lastName == null)
        //    {
        //        return new ResultCheckdb<Domain.Entities.Person>
        //        {
        //            IsSuccess = false,
        //            Error = "invalid input"
        //        };
        //    }


        //    var dbPerson = await _dbContext.People
        //        .FirstOrDefaultAsync(p => p.first_name == firstName && p.last_name == lastName);
        //    if (dbPerson == null)
        //    {
        //        return new ResultCheckdb<Domain.Entities.Person>
        //        {
        //            IsSuccess = true,
        //            IsFound = false,
        //            Error="person is not found"
        //        };
        //    }
        //    else
        //    {
        //        return new ResultCheckdb<Domain.Entities.Person>
        //        {
        //            IsSuccess = true,
        //            IsFound = true,
        //            Value = _personmapper.ToDomain(dbPerson)
        //        };
        //    }
        //}




        //public async Task<ResultCheckdb<Domain.Entities.ShopOwner> >GetShopOwnerByPersonAsync(Domain.Entities.Person person) 
        //{


        //        var dbShopOwner = await _dbContext.ShopOwners
        //                                      .FirstOrDefaultAsync(so => so.person_id == person.PersonId);
        //        if (dbShopOwner == null)
        //        {
        //            return new ResultCheckdb<Domain.Entities.ShopOwner>
        //            {
        //                IsSuccess = true,
        //                IsFound = false,
        //            };
        //        }
        //        else
        //        {
        //            return new ResultCheckdb<Domain.Entities.ShopOwner>
        //            {
        //                IsSuccess = true,
        //                IsFound = true,
        //                Value = _shopownermapper.ToDomain(dbShopOwner)
        //            };
        //        }




        //}
        //public async Task<ResultCheckdb<Domain.Entities.Admin>> GetAdminByPersonAsync(Domain.Entities.Person person)
        //{
        //    if (person == null) return new ResultCheckdb<Domain.Entities.Admin>
        //    {
        //        IsSuccess = false,
        //        Error = "Invalid person"
        //    };
        //    try
        //    {

        //        var dbadmin= await _dbContext.admains
        //                                      .FirstOrDefaultAsync(so => so.person_id == person.PersonId);
        //        if (dbadmin == null)
        //        {
        //            return new ResultCheckdb<Domain.Entities.Admin>
        //            {
        //                IsSuccess = true,
        //                IsFound = false,
        //            };
        //        }
        //        else
        //        {
        //            return new ResultCheckdb<Domain.Entities.Admin>
        //            {
        //                IsSuccess = true,
        //                IsFound = true,
        //                Value = _adminmapper.ToDomain(dbadmin)
        //            };
        //        }
        //    }

        //    catch (Exception ex)
        //    {
        //        return new ResultCheckdb<Domain.Entities.Admin>
        //        {
        //            IsSuccess = false,
        //            Error =ex.Message + " | " + ex.StackTrace
        //        };
        //    }

        //}
        //public async Task<ResultCheckdb<Domain.Entities.Person>> GetPersonByPersonId(int personid)
        //{
        //    try
        //    {
        //        var person =  _dbContext.People.Find(personid);
        //        if (person == null)
        //            return new ResultCheckdb<Domain.Entities.Person>
        //            {
        //                IsSuccess = true,
        //                IsFound = false,
        //                Error="the errorr in get person from database"

        //            };
        //        return new ResultCheckdb<Domain.Entities.Person>
        //        {
        //            IsSuccess = true,
        //            IsFound = true,
        //            Value =_personmapper.ToDomain(person)
        //        };


        //    }
        //    catch (Exception ex)
        //    {
        //        return new ResultCheckdb<Domain.Entities.Person>
        //        {
        //            IsSuccess = false,
        //            IsFound = false,
        //            Error=ex.Message

        //        };
        //    }
        //    }

        //public async Task<ResultCheckdb<Domain.Entities.Admin>> checkAdmainbypersonid(int personid)
        //{

        //    try
        //    {

        //        var admindb = _dbContext.admains.Find(personid);
        //        if(admindb == null)
        //        {
        //            return new ResultCheckdb<Admin>
        //            {
        //                IsSuccess = true,
        //                IsFound = false,
        //            };
        //        }

        //        return new ResultCheckdb<Admin>
        //        {
        //            IsSuccess = true,
        //            IsFound = true,
        //            Value = new Admin(admindb.admin_id, admindb.person_id.Value)

        //        };

        //    }catch(Exception ex)
        //    {
        //        return new ResultCheckdb<Admin>
        //        {
        //            IsSuccess = false,
        //            Error = ex.Message
        //        };
        //    }
        //}


        //public async Task<UpdateDataProcess> AddPersonAsync(Domain.Entities.Person person) {

        //    if (person == null)
        //        return UpdateDataProcess.yourdatanull;
        //    try
        //    {
        //        var dbPerson = _personmapper.ToEntity(person);
        //        _dbContext.People.Add(dbPerson);
        //        return UpdateDataProcess.Success;
        //    }
        //    catch (DbUpdateException ex)
        //    {
        //        Console.WriteLine("Database error: " + ex.Message);
        //        return UpdateDataProcess.catchError;
        //    }
        //    catch (Exception ex) {
        //        Console.WriteLine("Unexpected error of type"+ex.Message);
        //        Console.WriteLine("stack Trace"+ ex.StackTrace);
        //        if (ex.InnerException != null)
        //        {
        //            Console.WriteLine("Inner Exception" + ex.InnerException.Message);
        //        }
        //      return UpdateDataProcess.catchError;
        //    }
        //}

        //public async Task<UpdateDataProcess> AssignClientRoleToPersonAsync(Domain.Entities.Person person)
        //{
        //    if (person == null)
        //        return UpdateDataProcess.yourdatanull;
        //    var dbClient = new Infrastructure.Models.Client 
        //    {
        //        person_id = person.PersonId,

        //    };
        //    try
        //    {

        //        _dbContext.Clients.Add(dbClient);
        //        return UpdateDataProcess.Success;
        //    }
        //    catch (DbUpdateException ex) {
        //     Console.WriteLine(ex.Message);
        //        return UpdateDataProcess.catchError;
        //    }

        //}

        //public async Task<UpdateDataProcess> AssignShopOwnerRoleToPersonAsync(Domain.Entities.Person person) 
        //{
        //    if (person == null)
        //        return UpdateDataProcess.yourdatanull;
        //    var dbShopOwner = new Infrastructure.Models.ShopOwner 
        //    {
        //        person_id = person.PersonId,

        //    };
        //    try
        //    {
        //        _dbContext.ShopOwners.Add(dbShopOwner);
        //        return UpdateDataProcess.Success;
        //    }
        //    catch (DbUpdateException ex) { 
        //        return UpdateDataProcess.catchError;
        //    }
        //}
        //public async Task<UpdateDataProcess> AssignAdmintRoleToPersonAsync(Domain.Entities.Person person)
        //{
        //    if (person == null)
        //        return UpdateDataProcess.yourdatanull;
        //    var dbAdmin = new Infrastructure.Models.admain
        //    {
        //        person_id = person.PersonId,

        //    };
        //    try
        //    {

        //        _dbContext.admains.Add(dbAdmin);
        //        return UpdateDataProcess.Success;
        //    }
        //    catch (DbUpdateException ex)
        //    {
        //        Console.WriteLine(ex.Message);
        //        return UpdateDataProcess.catchError;
        //    }

        //}



    }
}
