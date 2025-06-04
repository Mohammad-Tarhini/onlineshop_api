using onlineshopowner_api.Application.Dtos;
using onlineshopowner_api.Application.Interfaces.Ivalidator;
using onlineshopowner_api.Infrastructure.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using onlineshopowner_api.Application.Interfaces;
using onlineshopowner_api.Application.Interfaces.Iservices;
using onlineshopowner_api.Domain.Interfaces.IRepository;
using Microsoft.Ajax.Utilities;
using System.Runtime.Remoting.Messaging;

namespace onlineshopowner_api.Application.Validatorandclean
{
    public  class Registerationhelper: Iregistrationhelper
    {
        private readonly IUnityOfWork _unitOfWork;

        public Registerationhelper(IUnityOfWork unityOfwork)
        {
            _unitOfWork = unityOfwork;
        }
        public async Task<(bool IsSuccess,bool isfound, Domain.Entities.Person person,string message)> foundingperson(RegisterationRequestDto dto)
        {
            this.ValidateInput(dto);
            
            if (dto.email != null)
            {
                var theexistperson1 = await _unitOfWork.PersonRepository.GetPersonByEmailAsync(dto.email);
                if (theexistperson1.IsSuccess == false)
                {
                    return (false, false, null,theexistperson1.Error);
                }
                if (theexistperson1.IsSuccess == true)
                {
                    if (theexistperson1.IsFound == true)
                    {
                        return (true, true, theexistperson1.Value,theexistperson1.Error);
                    }
                    
                }
            }
            if (dto.phonenumber != null)
            {
                var theexistperson2 = await _unitOfWork.PersonRepository.GetPersonByPhoneNumberAsync(dto.phonenumber);
                if (theexistperson2.IsSuccess == false)
                {
                    return (false, false, null,theexistperson2.Error);

                }
                else
                {
                    if (theexistperson2.IsFound)
                    {
                        return (true, true, theexistperson2.Value,theexistperson2.Error);
                    }
                }
            }
           var theexistperson3 = await _unitOfWork.PersonRepository.GetPersonByCredentialsAsync(dto.first_name, dto.last_name );
            if (theexistperson3.IsSuccess == false)
            {
                return (false, false, null,theexistperson3.Error);
            }
            if (theexistperson3.IsSuccess == true)
            {
                if (theexistperson3.IsFound == true)
                {
                    return (true, true, theexistperson3.Value,theexistperson3.Error);
                }
            }
           
         var   personToProcess = new Domain.Entities.Person(
    firstname: dto.first_name, lastname: dto.last_name, email: dto.email, sex: dto.sex,
     phonenumber: dto.phonenumber, password: HashingPassword.HashPassword(dto.password));
            try { 
                await _unitOfWork.PersonRepository.AddPersonAsync(personToProcess);
                await _unitOfWork.CommitAsync();
            }
            catch (Exception ex) { return (false, false, null,"error in RSH M1"); };

            return (true,false,personToProcess,theexistperson3.Error);



        }
        public async Task<(bool IsSuccess, bool arleadyExistClient, string message)>Registeronclient(bool arleadexistperson,Domain.Entities.Person personToProcess)
        {
          
            try { 
           
                if (!arleadexistperson)
                {
                    try
                    {
                        await _unitOfWork.PersonRepository.AssignClientRoleToPersonAsync(personToProcess);
                        await _unitOfWork.CommitAsync();
                        return (true,false, null);
                    }
                    catch (Exception ex)
                    {
                        return (false,false, "error RSM m2 1");
                    }
                }
                else
                {
                    var client = await _unitOfWork.PersonRepository.GetClientByPersonAsync(personToProcess);
                    if (!client.IsSuccess)
                    {
                        return (false,false, client.Error);
                    }
                    if (client.IsFound)
                    {
                        return (true ,true ,client.Error);
                    }
                    try
                    {
                        await _unitOfWork.PersonRepository.AssignClientRoleToPersonAsync(personToProcess);
                        await _unitOfWork.CommitAsync();

                        return (true,false, "New client registered successfully.");
                    }
                    catch (Exception ex) { return (false,false, "error RSH M2 2"); }
                   

                }
            }
            catch (Exception ex) {
                return (false,false, ex.Message);
            }
        }
        public async Task<(bool IsSuccess,bool arleadyExistShopOwner ,string message)> RegisterShopOwner(bool arleadexistperson, Domain.Entities.Person personToProcess)
        {
            try
            {

                if (!arleadexistperson)
                {
                    try
                    {
                      await _unitOfWork.PersonRepository.AssignShopOwnerRoleToPersonAsync(personToProcess);
                        return (true,false ,null);
                    }
                    catch (Exception ex)
                    {
                        return (false, false,ex.Message);
                    }
                }
                else
                {
                    var client = await _unitOfWork.PersonRepository.GetShopOwnerByPersonAsync(personToProcess);
                    if (!client.IsSuccess)
                    {
                        return (false,false ,client.Error);
                    }
                    if (client.IsFound)
                    {
                        return (true,true ,client.Error);
                    }
                    try
                    {
                        await _unitOfWork.PersonRepository.AssignShopOwnerRoleToPersonAsync(personToProcess);
                        return (true,false ,"New client registered successfully.");
                    }
                    catch (Exception ex) { return (false, false,ex.Message); }


                }
            }
            catch (Exception ex)
            {
                return (false,false ,ex.Message);
            }

        }

    





    public void ValidateInput(RegisterationRequestDto dto)
        {

            dto.CleanStrings();
            if (string.IsNullOrWhiteSpace(dto.role))
                throw new ArgumentNullException(nameof(dto.role), "role can not be null");
            if (dto.role != "client" && dto.role != "shopowner")
            {
                throw new ArgumentException(nameof(dto.role), "role should be client or shopowner");
            }

        }
    }
}
   