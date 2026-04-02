using onlineshopowner_api.Application.Dtos;
using onlineshopowner_api.Application.Interfaces.Iservices;
using onlineshopowner_api.Application.Interfaces.Itoken;
using onlineshopowner_api.Application.Validatorandclean;
using onlineshopowner_api.Infrastructure.Token;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace onlineshopowner_api.Application.Services.AuthoServices
{
    public class LoginServices:ILoginService
    {
        private readonly IUnityOfWork unityOfWork;
        private readonly IjwtTokenGenerator jwtTokenGenerator;

        public LoginServices(IUnityOfWork unityOfWork , IjwtTokenGenerator jwtTokenGenerator)
        {
           this. unityOfWork = unityOfWork;
            this.jwtTokenGenerator = jwtTokenGenerator;
        }
        public async  Task<string> Login(LoginRequestDto dto)
        {
            if(dto.email ==null && dto.phonenumber == null)
            {

                throw new Exception("");
            } 
            var person= await unityOfWork.PersonRepository.GetPersonByEmailOrPhonenumber(dto.email,dto.phonenumber);
            if(person== null)
            {
                throw new Exception("sory this user is not exist ");

            }
           await this.RoleVerify(dto.role, person.Id);
            dto.password = dto.password?.Trim();
            
            if (!HashingPassword.VerifyPassword(dto.password, person.Password))
            {
                throw new Exception("invalid password ");
            }
            var token = jwtTokenGenerator.GenerateToken(person.Id,dto.role,50);
            return token;

        }
        public async Task RoleVerify(string role , int personId)
        {
            if(role==null)
                throw new Exception();
            role=role.ToLower().Trim();
            if(role !="client" &&  role !="shopowner" &&role != "delivery")
            {
                throw new Exception("enter the correct role ");
            }
            if (role == "client" && await unityOfWork.PersonRepository.GetClientIdByPersonId(personId) == null)
            {
                throw new Exception(" sorry the user is not client ");
            }
            if (role == "shopowner" && await unityOfWork.PersonRepository.GetShopOwnerIdByPersonId(personId) == null)
            {
                throw new Exception("sorry the user is not shopowner ");
            }
            if (role == "delivery" && await unityOfWork.PersonRepository.GetDeliveryIdByPersonId(personId) == null)
                {
                    throw new Exception("sorry the user is not delivery ");
            }

        }
    }
}