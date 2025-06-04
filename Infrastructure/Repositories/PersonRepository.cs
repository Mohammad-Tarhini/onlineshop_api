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
using onlineshopowner_api.Domain.Constant;
using onlineshopowner_api.Domain.Entities;

namespace onlineshopowner_api.Infrastructure.Repositories
{
    public class PersonRepository:IpersonRepository
    {
        private readonly online_shopEntities _dbContext;
        private readonly IMapper<Domain.Entities.Person, Models.Person> _personmapper;
        private readonly IMapper<Domain.Entities.ShopOwner, Models.ShopOwner> _shopownermapper;
        private readonly IMapper<Domain.Entities.Client, Models.Client> _clientmapper;
        public PersonRepository(online_shopEntities dbContext, IMapper<Domain.Entities.Person, Models.Person> personmapper,IMapper<Domain.Entities.Client,Models.Client> clientmapper,IMapper<Domain.Entities.ShopOwner,Models.ShopOwner> shopownermapper)
        {
            _dbContext = dbContext;
            _personmapper = personmapper;
            _clientmapper = clientmapper;
            _shopownermapper=shopownermapper;
        }

        public async Task<ResultCheckdb<Domain.Entities.Person>> GetPersonByEmailAsync(string email)
        {
            if (email == null)
            {
                return new ResultCheckdb<Domain.Entities.Person>
                {
                    IsSuccess = false,
                    Error = "email not exist"
                };
            }
            try
            {
                var dbPerson = await _dbContext.People
                    .FirstOrDefaultAsync(p => p.email == email);
                if (dbPerson == null)
                {
                    return new ResultCheckdb<Domain.Entities.Person>
                    {
                        IsSuccess = true,
                        IsFound = false,
                        Error = "person is NOT    found"
                    };
                }
                else
                {
                    return new ResultCheckdb<Domain.Entities.Person>
                    {
                        IsSuccess = true,
                        IsFound = true,
                        Value=_personmapper.ToDomain(dbPerson)
                    };
                }
               
            }
            catch (Exception ex)
            {
                return new ResultCheckdb<Domain.Entities.Person>
                {
                    IsSuccess = false,
                    Error = ex.Message
                };
                
            }
        }

        public async Task<ResultCheckdb<Domain.Entities.Person> >GetPersonByPhoneNumberAsync(string phoneNumber)
        {if (phoneNumber == null)
            {
                return new ResultCheckdb<Domain.Entities.Person>
                {
                    IsSuccess = false,
                    Error = "Invalid phone number"
                };
            }
            try
            {
                var dbPerson = await _dbContext.People
                    .FirstOrDefaultAsync(p => p.phone_number == phoneNumber);
                if (dbPerson == null)
                {
                    return new ResultCheckdb<Domain.Entities.Person>
                    {
                        IsSuccess = true,
                        IsFound = false,
                        Error="person is not found"
                    };
                }
                else
                {
                    return new ResultCheckdb<Domain.Entities.Person>
                    {
                        IsSuccess = true,
                        IsFound = true,
                        Value = _personmapper.ToDomain(dbPerson)
                    };
                }
            }
            catch (Exception ex) {
                return new ResultCheckdb<Domain.Entities.Person> { 
                IsSuccess=false,
                Error = ex.Message
                };
            }

        }

        public async Task<ResultCheckdb<Domain.Entities.Person>> GetPersonByCredentialsAsync(string firstName, string lastName) 
        {if (firstName == null || lastName == null)
            {
                return new ResultCheckdb<Domain.Entities.Person>
                {
                    IsSuccess = false,
                    Error = "invalid input"
                };
            }


            var dbPerson = await _dbContext.People
                .FirstOrDefaultAsync(p => p.first_name == firstName && p.last_name == lastName);
            if (dbPerson == null)
            {
                return new ResultCheckdb<Domain.Entities.Person>
                {
                    IsSuccess = true,
                    IsFound = false,
                    Error="person is not found"
                };
            }
            else
            {
                return new ResultCheckdb<Domain.Entities.Person>
                {
                    IsSuccess = true,
                    IsFound = true,
                    Value = _personmapper.ToDomain(dbPerson)
                };
            }
        }

        public async Task<ResultCheckdb<Domain.Entities.Client>> GetClientByPersonAsync(Domain.Entities.Person person) 
        {
            if (person == null)  return new ResultCheckdb<Domain.Entities.Client>
            {
                IsSuccess = false,
                Error="no inputs"
            };
            try
            {
                var dbClient = await _dbContext.Clients.FirstOrDefaultAsync(c => c.person_id == person.PersonId);
                if (dbClient == null)
                {
                    return new ResultCheckdb<Domain.Entities.Client>
                    {
                        IsSuccess = true,
                        IsFound = false,
                        Error = "This person is already registered as a client."

                    };
                }
                else
                {
                    return new ResultCheckdb<Domain.Entities.Client>
                    {
                        IsSuccess = true,
                        IsFound = true,
                        Value = _clientmapper.ToDomain(dbClient)
                    };
                }
            }
            catch (Exception ex) { 
                return new ResultCheckdb<Domain.Entities.Client> { 
                IsSuccess= false,
                Error=ex.Message
                };
            }
        }

        public async Task<ResultCheckdb<Domain.Entities.ShopOwner> >GetShopOwnerByPersonAsync(Domain.Entities.Person person) 
        {
            if (person == null) return new ResultCheckdb<Domain.Entities.ShopOwner> { 
            IsSuccess = false,
            Error= "Invalid person"
            };
            try
            {

                var dbShopOwner = await _dbContext.ShopOwners
                                              .FirstOrDefaultAsync(so => so.person_id == person.PersonId);
                if (dbShopOwner == null)
                {
                    return new ResultCheckdb<Domain.Entities.ShopOwner>
                    {
                        IsSuccess = true,
                        IsFound = false,
                    };
                }
                else
                {
                    return new ResultCheckdb<Domain.Entities.ShopOwner>
                    {
                        IsSuccess = true,
                        IsFound = true,
                        Value = _shopownermapper.ToDomain(dbShopOwner)
                    };
                }
            }

            catch (Exception ex) {
                return new ResultCheckdb<Domain.Entities.ShopOwner>
                {
                    IsSuccess = false,
                    Error = ex.Message
                };
            }

        }

        public async Task<UpdateDataProcess> AddPersonAsync(Domain.Entities.Person person) {

            if (person == null)
                return UpdateDataProcess.yourdatanull;
            try
            {
                var dbPerson = _personmapper.ToEntity(person);
                _dbContext.People.Add(dbPerson);
                return UpdateDataProcess.Success;
            }
            catch (DbUpdateException ex)
            {
                Console.WriteLine("Database error: " + ex.Message);
                return UpdateDataProcess.catchError;
            }
            catch (Exception ex) {
                Console.WriteLine("Unexpected error of type"+ex.Message);
                Console.WriteLine("stack Trace"+ ex.StackTrace);
                if (ex.InnerException != null)
                {
                    Console.WriteLine("Inner Exception" + ex.InnerException.Message);
                }
              return UpdateDataProcess.catchError;
            }
        }

        public async Task<UpdateDataProcess> AssignClientRoleToPersonAsync(Domain.Entities.Person person)
        {
            if (person == null)
                return UpdateDataProcess.yourdatanull;
            var dbClient = new Infrastructure.Models.Client 
            {
                person_id = person.PersonId,

            };
            try
            {

                _dbContext.Clients.Add(dbClient);
                return UpdateDataProcess.Success;
            }
            catch (DbUpdateException ex) {
             Console.WriteLine(ex.Message);
                return UpdateDataProcess.catchError;
            }

        }

        public async Task<UpdateDataProcess> AssignShopOwnerRoleToPersonAsync(Domain.Entities.Person person) 
        {
            if (person == null)
                return UpdateDataProcess.yourdatanull;
            var dbShopOwner = new Infrastructure.Models.ShopOwner 
            {
                person_id = person.PersonId,
                
            };
            try
            {
                _dbContext.ShopOwners.Add(dbShopOwner);
                return UpdateDataProcess.Success;
            }
            catch (DbUpdateException ex) { 
                return UpdateDataProcess.catchError;
            }
        }
    }
}
