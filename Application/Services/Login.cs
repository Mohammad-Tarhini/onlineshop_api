using onlineshopowner_api.Application.Dtos;
using onlineshopowner_api.Application.Interfaces.Iservices;
using onlineshopowner_api.Application.Interfaces.Itoken;
using onlineshopowner_api.Application.Interfaces.Ivalidator;
using onlineshopowner_api.Application.Validatorandclean;
using onlineshopowner_api.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using System.Web;

namespace onlineshopowner_api.Application.Services
{

    public class login
    {
        private readonly ILoginhelper _loginhelper;
        private readonly IUnityOfWork _unitOfWork;
        private readonly IjwtTokenGenerator _tokenGenerator;

        public login(ILoginhelper loginhelper, IUnityOfWork unityofwork, IjwtTokenGenerator token)
        {
            _loginhelper = loginhelper;
            _unitOfWork = unityofwork;
            _tokenGenerator = token;
        }
        public async Task<(bool IsSuccess, string Token, string error)> LoginClientOrShopowner(LoginRequestDto Dto, string role)
        {
            try
            {

                bool IsFound;
                Domain.Entities.Person person = null;
                string message;
                if (Dto.phonenumber != null)
                {
                    if(int.TryParse(Dto.phonenumber,out int number)){ 
                        return (false, null, "phone number wrong");
                    }
                    try
                    {
                        (IsFound, person, message) = await _loginhelper.CheckExistPersonByPhoneNumber(Dto);
                        if (!IsFound)
                        {
                            return (false, null, message);
                        }
                    }
                    catch (Exception ex)
                    {
                        return (false, null, ex.Message);

                    }
                }
                if (Dto.email != null)
                {
                    try
                    {
                        (IsFound, person, message) = await _loginhelper.CheckExistPersonByEmail(Dto);
                        if (!IsFound)
                        {
                            return (false, null, message);
                        }
                    }
                    catch (Exception ex)
                    {
                        return (false, null, ex.Message);
                    }
                }

                if (role == "client")
                {
                    try
                    {
                        var ResultCheckClient = await _unitOfWork.PersonRepository.GetClientByPersonAsync(person);
                        if (!ResultCheckClient.IsFound)
                        {
                            return (false, null, ResultCheckClient.Error);
                        }
                        if (!HashingPassword.VerifyPassword(Dto.password, person.Password))
                        {
                            return (false, null, "the password is wrong ");
                        }

                    }
                    catch (Exception ex) { return (false, null, ex.Message); }
                }
                else if (role == "shopowner")
                {
                    try
                    {

                        var ResultCheckShopOwner = await _unitOfWork.PersonRepository.GetShopOwnerByPersonAsync(person);
                        if (!ResultCheckShopOwner.IsFound)
                        {
                            return (false, null, ResultCheckShopOwner.Error);
                        }
                        if (!HashingPassword.VerifyPassword(Dto.password, person.Password))
                        {
                            return (false, null, "the password is wrong ");
                        }
                    }catch(Exception ex) {
                        return (false, null, ex.Message);
                    }

                }
                string token = _tokenGenerator.GenerateToken(person.PersonId, "client", 60);
                return (true, token, "success");
            }
            catch (Exception ex)
            {
                return(false, null,ex.Message);
            }
        } 
            
    }
}