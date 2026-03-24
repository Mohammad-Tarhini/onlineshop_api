5using onlineshopowner_api.Application.Dtos;
using onlineshopowner_api.Application.Interfaces.Iservices;
using onlineshopowner_api.Application.Validatorandclean;
using onlineshopowner_api.Domain.Entities;
using onlineshopowner_api.Domain.Entities.Delivery;
using onlineshopowner_api.Domain.Interfaces.IExternalServices;
using onlineshopowner_api.Infrastructure.ExternalServices.comunicate;
using onlineshopowner_api.Infrastructure.Models;
using onlineshopowner_api.Infrastructure.OnException;
using onlineshopowner_api.Infrastructure.Repositories;
using onlineshopowner_api.Infrastructure.Token;
using StackExchange.Redis;
using System.Collections.Generic;
using System.Data.Entity.Core.Metadata.Edm;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http.Results;
using onlineshopowner_api.Application.Dtos.DeliveryDtos;
using Microsoft.Ajax.Utilities;

namespace onlineshopowner_api.Application.Services.AuthoServices
{
    
    public class RegisterationService:IRegisterationServices
    {
        private readonly IUnityOfWork uow;
        
        private readonly ITwilioMessageService TwilioMessageService;
        private readonly IEmailService EmailService;
        
        public RegisterationService(
            IUnityOfWork uow
           ,ITwilioMessageService twilioMessageService,IEmailService emailService)
        {
            this.uow = uow;
           
            this.TwilioMessageService = twilioMessageService;
            this.EmailService = emailService;
        }
        public async Task RegisterClient(RegistrationRequestDto dto)
        {
           if(dto.role.ToLower().Trim() != "client")
           {
               throw new DomainException("Role must be client for this method");
           }
            var person=await uow.PersonRepository
                .GetPersonByEmailOrPhonenumber(dto.personDto.Email, dto.personDto.PhoneNumber);
            if(person != null)
            {
                var clientId = await uow.PersonRepository.GetClientIdByPersonId(person.Id);
                if (clientId != null)
                {
                    throw new DomainException("Already registered as client");
                }
                else
                {
                    var client = new Domain.Entities.Client {PersonId=person.Id };
                    await uow.PersonRepository.AddClientByPerson(client);
                    await uow.CommitAsync();
                    return;
                }
            }
            string token=VerificationTokenLink.GenerateToken(dto.personDto, dto.role);
            string verificationLink =
    $"https://localhost:44364//api/Client/verifyregisteration?token={token}";
            string body = $@"
<h2>Verify your account</h2>
<p>Click the button below to verify your account.</p>

<a href='{verificationLink}' 
style='padding:10px 20px;background-color:#4CAF50;color:white;text-decoration:none;border-radius:5px'>
Verify Account
</a>

<p>If the button does not work, copy this link:</p>
<p>{verificationLink}</p>
";

            if (string.IsNullOrEmpty(token)) 
            {
                throw new DomainException("Error generating verification token");
            }
            if(!string.IsNullOrEmpty(dto.personDto.Email))
            {
              await  EmailService.SendEmailAsync(dto.personDto.Email,"account verifaction", body);
            }
            else if(!string.IsNullOrEmpty(dto.personDto.PhoneNumber))
            {
              await  TwilioMessageService.SendSmsAsync(dto.personDto.PhoneNumber, token);
            }
            else
            {
                throw new DomainException("No email or phone number provided");
            }

        }

        public async Task VerifyRegisteration(string token)
        {
            if (!VerificationTokenLink.ValidateToken(token, out PersonDto personDto, out string role))
            {
                throw new DomainException("Invalid or expired token");
            }
            var checkPerson = await uow.PersonRepository.GetPersonByEmailOrPhonenumber(personDto.Email, personDto.PhoneNumber);
            int personId;
            if (checkPerson != null)
            {
                var clientId = await uow.PersonRepository.GetClientIdByPersonId(checkPerson.Id);
                if (clientId != null)
                {
                    throw new DomainException("Already registered as client");
                }
                personId = checkPerson.Id;
            }
            else
            {
                var personD = new Domain.Entities.Person(
                                personDto.FirstName, personDto.LastName, personDto.Email, personDto.Sex, personDto.PhoneNumber, personDto.Password);

                await uow.PersonRepository.AddPersonAsync(personD);
                personId=personD.Id;
                //await uow.CommitAsync();
            }

           
           await uow.PersonRepository.AddClientByPerson(new Domain.Entities.Client { PersonId = personId });
            await uow.CommitAsync();
        }


        public async Task RegisterShopOwnerorDelivery(RegistrationRequestDto dto)
        {
            if(dto.role.ToLower().Trim() != "shopowner" && dto.role.ToLower().Trim() != "delivery")
            {
                throw new DomainException("Role must be shopowner for this method");
            }
            var persondb=await uow.PersonRepository
                .GetPersonByEmailOrPhonenumber(dto.personDto.Email, dto.personDto.PhoneNumber);
                if (persondb != null && ((dto.role.ToLower().Trim() == "shopowner" && uow.PersonRepository.GetShopOwnerIdByPersonId(persondb.Id) != null) || (dto.role.ToLower().Trim() == "delivery" && uow.PersonRepository.GetDeliveryIdByPersonId(persondb.Id) != null)))
            {
                throw new DomainException($"Already registered as {dto.role}");
            }

                string otp=OtpService.GenerateOtp();
             var person= new Domain.Entities.Person(
                            dto.personDto.FirstName, dto.personDto.LastName, dto.personDto.Email, dto.personDto.Sex, dto.personDto.PhoneNumber,HashingPassword.HashPassword( dto.personDto.Password));

            if (dto.role.ToLower().Trim() == "shopowner")
            {
                
                await uow.PersonRepository.AddToPandingRegisteration(person, otp, "shopowner");
            }
            else if(dto.role.ToLower().Trim() == "delivery")
            {
                await uow.PersonRepository.AddToPandingRegisteration(person, otp, "delivery");
            }
            else
            {
                throw new DomainException("Role must be shopowner or delivery for this method");
            }
                await uow.CommitAsync();
            if (!string.IsNullOrEmpty(dto.personDto.Email))
            {
              await  EmailService.SendEmailAsync(dto.personDto.Email, "otp code for verification", otp);
            }
            else
            {
                await TwilioMessageService.SendSmsAsync(dto.personDto.PhoneNumber, otp);
            }
            

        }
        public async Task AddVerifiedShopowner(VerifyOtpDto verifyOtpDto)
        {
            if(verifyOtpDto.phoneNumber ==null && verifyOtpDto.email == null)
            {
                throw new DomainException("there is no phone number and email");
            }
            var temData = OtpService.VerifyOtp(verifyOtpDto.Otp, verifyOtpDto.email, verifyOtpDto.phoneNumber);
            if(temData.Role.ToLower().Trim() != "shopowner")
            {
                throw new DomainException("sory you are not register as shopowner ");
            }
            var checkPerson = await uow.PersonRepository.GetPersonByEmailOrPhonenumber(temData.Email, temData.PhoneNumber);
            int personId;
            if (checkPerson != null) 
            {
                if(await uow.PersonRepository.GetShopOwnerIdByPersonId(checkPerson.Id) != null)
                {
                    throw new DomainException("");
                }
                personId= checkPerson.Id;
            }
            else
            {
                var person = new Domain.Entities.Person(temData.FirstName, temData.LastName, temData.Email, temData.Sex, temData.PhoneNumber, temData.PasswordHash);
                await uow.PersonRepository.AddPersonAsync(person);
                personId = person.Id;
            }

               
          
          


                var shopowner = new Domain.Entities.ShopOwner { PersonId = personId };
            await uow.PersonRepository.AddShopOwnerByPerson(shopowner);
            //await uow.CommitAsync();
           await uow.PersonRepository.DeletePendingPerson(verifyOtpDto.email, verifyOtpDto.phoneNumber);
            await uow.CommitAsync();


        }
        
        public async Task AddVerifiedDelivery(DeliveryProviderDto deliveryProviderDto,VerifyOtpDto verifyOtpDto)
        {
            if (verifyOtpDto.phoneNumber == null && verifyOtpDto.email == null)
            {
                throw new DomainException("there is no phone number and email");
            }
            var temData = OtpService.VerifyOtp(verifyOtpDto.Otp, verifyOtpDto.email, verifyOtpDto.phoneNumber);
            if(temData == null)
            {
                throw new DomainException("Invalid OTP");
            }
            var checkPerson=await uow.PersonRepository.GetPersonByEmailOrPhonenumber(verifyOtpDto.email,verifyOtpDto.phoneNumber);
            int personId;
            if(checkPerson != null)
            {
                personId = checkPerson.Id;
                var checkDeliveryId = await uow.PersonRepository.GetDeliveryIdByPersonId(personId);
                if (checkDeliveryId != null ) 
                {
                    throw new DomainException("this delivery is arleady exist ");
                }
            }
            else
            {
                var person = new Domain.Entities.Person(temData.FirstName, temData.LastName, temData.Email, temData.Sex, temData.PhoneNumber, temData.PasswordHash);
                await uow.PersonRepository.AddPersonAsync(person);
                personId=person.Id;

            }


            // await uow.CommitAsync();
            // int personId = person.Id;
            if (deliveryProviderDto == null)
            {
                throw new DomainException("Delivery data is required");
            }
            var deliveryProvider = new Domain.Entities.Delivery.DeliveryProvider
            {
                provider_type = deliveryProviderDto.provider_type,
                active_bit = deliveryProviderDto.active_bit,


                person_id = personId,
                note_text = deliveryProviderDto.note_text


            };
          await  uow.DelivaryRepository.AddDeliveryProvider(deliveryProvider);
            int delivery_id = deliveryProvider.Delivery_id;
            //   await uow.CommitAsync();

            if (deliveryProviderDto.workHours != null)
            {
                foreach (var item in deliveryProviderDto.workHours)
                {

                    var deliveryWorkigHours = new Domain.Entities.Delivery.DeliveryWorkigHours
                    {
                        WeekDay = item.WeekDay,
                        Open_time = item.Open_time,
                        Close_time = item.Close_time,
                        DeliveryId = delivery_id,
                    };
                    await uow.DelivaryRepository.AddDeliveryWorkingHour(deliveryWorkigHours);
                    List<string> regions = new List<string>();
                }
            }
            if (deliveryProviderDto.regionnames != null)
            {
                foreach (var item in deliveryProviderDto.regionnames)
                {
                    var regionId = await uow.DelivaryRepository.GetRegionIdByRegionName(item);
                    if (regionId == null)
                    {
                        //regionId = await uow.RegionRepository.AddRegion(new Domain.Entities.Region { Name = item });
                        //await uow.CommitAsync();
                        throw new DomainException($"Region {item} does not exist");
                    }
                    await uow.DelivaryRepository.AddDeliveryRegion(new Domain.Entities.Delivery.DeliveryRegion { DeliveryId = delivery_id, RegionId = regionId.Value });

                }
            }
           await uow.CommitAsync();

             

        }
        //    public async Task Register(RegistrationRequestDto dto)
        //    {
        //        using (var transaction = await uow.BeginTransaction())
        //        {
        //            try
        //            {



        //                var personDto = dto.personDto;
        //                var roleDto = dto.role.ToLower().Trim();
        //                int personId;
        //                var person = await
        //                    uow.PersonRepository
        //                       .GetPersonByEmailOrPhonenumber(personDto.Email, personDto.PhoneNumber);

        //                if (person != null)
        //                {
        //                    this.ValidateRole(person.Id, roleDto);
        //                    personId = person.Id;
        //                }
        //                else
        //                {
        //                    if (roleDto == "shopowner" || roleDto == "delivery" || roleDto == "admin")
        //                    {
        //                        //here there logic to send this data to the admin to to ask him if allow
        //                        return;
        //                    }

        //                    else
        //                    {
        //                        var personD = new Domain.Entities.Person(
        //                            personDto.FirstName, personDto.LastName, personDto.Email, personDto.Sex, personDto.PhoneNumber, personDto.Password);

        //                        await uow.PersonRepository.AddPersonAsync(personD);
        //                        await uow.CommitAsync();

        //                        personId = personD.Id;

        //                    }
        //                }
        //                AddRole(personId, roleDto);
        //                await uow.CommitAsync();


        //            } catch
        //{
        //                transaction.Rollback();
        //                throw;
        //            }



        //        }
        //    }
        //    private void AddRole(int personId,string role)
        //    {
        //        switch (role)
        //        {
        //            case "client":
        //                uow.PersonRepository.AddClientByPerson(new Domain.Entities.Client { PersonId = personId });
        //                break;

        //            case "shopowner":
        //                uow.PersonRepository.AddShopOwnerByPerson(new Domain.Entities.ShopOwner { PersonId = personId });
        //                break;

        //            //case "delivery":
        //            //    uow.PersonRepository.Add(new Delivery { PersonId = personId });
        //            //    break;

        //            case "admin":
        //                uow.PersonRepository.AddAdminByPerson(new Domain.Entities.Admin { personid = personId });
        //                break;
        //        }
        //    }

        //    private void ValidateRole(int personId, string role)
        //    {
        //        switch (role)
        //        {
        //            case "client":
        //                if (uow.PersonRepository.GetClientIdByPersonId(personId) != null)
        //                    throw new DomainException("Already registered as client");
        //                break;

        //            case "shopowner":
        //                if (uow.PersonRepository.GetShopOwnerIdByPersonId(personId) != null)
        //                    throw new DomainException("Already registered as shop owner");
        //                break;

        //            //case "delivery":
        //            //    if (uow.PersonRepository.GetDeliveryIdByPersonId(personId) != null)
        //            //        throw new DomainException("Already registered as delivery");
        //            //    break;

        //            case "admin":
        //                if (uow.PersonRepository.GetAdminIdByPersonId(personId) != null)
        //                    throw new DomainException("Already registered as admin");
        //                break;
        //        }
        //    }
    }

}