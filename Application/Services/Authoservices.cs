using onlineshopowner_api.Application.Dtos;
using onlineshopowner_api.Application.Dtos.DeliveryDtos;
using onlineshopowner_api.Application.Interfaces.Iservices;
using onlineshopowner_api.Application.Interfaces.Itoken;
using onlineshopowner_api.Application.Interfaces.Ivalidator;
using onlineshopowner_api.Application.Validatorandclean;
using onlineshopowner_api.Domain.Constant;
using onlineshopowner_api.Infrastructure.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace onlineshopowner_api.Application.Services
{
    public class AuthoServices:IAuthoServices
    {
        private readonly IUnityOfWork _unitOfWork;
        private readonly IjwtTokenGenerator _tokenGenerator;
        private readonly IAuthHelper _authHelper;
        public AuthoServices(IAuthHelper authHelper , IUnityOfWork unitOfWork, IjwtTokenGenerator tokenGenerator)
        {
           
            _unitOfWork = unitOfWork;
            _tokenGenerator = tokenGenerator;
            _authHelper=authHelper;
        }
        public async Task<(bool IsSuccess, string Token, string error)> LoginClientOrShopownerOrAdmin(LoginRequestDto Dto, string role)
        {
            try
            {

                bool IsFound;
                Domain.Entities.Person person = null;
                string message;
                if (Dto.phonenumber != null)
                {

                    try
                    {
                        (IsFound, person, message) = await _authHelper.CheckExistPersonByPhoneNumber(Dto);
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
                        (IsFound, person, message) = await _authHelper.CheckExistPersonByEmail(Dto);
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

                    }
                    catch (Exception ex)
                    {
                        return (false, null, ex.Message);
                    }


                }
                else if (role == "admin")
                {
                    try
                    {

                        var ResultCheckAdmin = await _unitOfWork.PersonRepository.GetAdminByPersonAsync(person);
                        if (!ResultCheckAdmin.IsFound)
                        {
                            return (false, null, ResultCheckAdmin.Error);
                        }

                    }
                    catch (Exception ex)
                    {
                        return (false, null, ex.Message);
                    }


                }
                else return (false, null, "no role");

                if (!HashingPassword.VerifyPassword(Dto.password, person.Password))
                {
                    return (false, null, "the password is wrong ");
                }
                string token = _tokenGenerator.GenerateToken(person.PersonId, role, 60);
                return (true, token, "success");
            }
            catch (Exception ex)
            {
                return (false, null, ex.Message);
            }
        }



        public async Task<(bool IsSuccess, bool alreadyexist, string Token, string Message)> RegisterAsync(RegisterationRequestDto dto)
        {


            try
            {
                var (issuccess, isfound, personToProcess, message) = await _authHelper.foundingperson(dto);


                if (!issuccess)
                {
                    return (false, false, null, message);
                }

                string roleToAssign = dto.role.ToLowerInvariant();


                if (roleToAssign == UserRoles.Client)
                {
                    try
                    {
                        var (IsSucces1, ClientAlreadyExist, message1) = await _authHelper.Registeronclient(isfound, personToProcess);
                        if (!IsSucces1)
                        {
                            return (false, false, null, message1);
                        }
                        var token = _tokenGenerator.GenerateToken(personToProcess.PersonId, roleToAssign, 60);



                        return (true, ClientAlreadyExist, token, message1);
                    }
                    catch (Exception ex)
                    {
                        return (false, false, null, ex.Message);
                    }

                }
                else if (roleToAssign == UserRoles.ShopOwner)
                {
                    try
                    {
                        var (IsSucces1, ShopOwnerAlreadyExist, message1) = await _authHelper.RegisterShopOwner(isfound, personToProcess);

                        await _unitOfWork.CommitAsync();

                        var token = _tokenGenerator.GenerateToken(personToProcess.PersonId, roleToAssign, 60);
                        return (IsSucces1, ShopOwnerAlreadyExist, token, message1);
                    }
                    catch (Exception ex)
                    {
                        return (false, false, null, ex.Message);
                    }

                }
                else if (roleToAssign == UserRoles.Admin)
                {

                    try
                    {
                        var (IsSucces1, admainalreadyexist, message1) = await _authHelper.Registeradmin(isfound, personToProcess);

                        await _unitOfWork.CommitAsync();

                        var token = _tokenGenerator.GenerateToken(personToProcess.PersonId, roleToAssign, 60);
                        return (IsSucces1, admainalreadyexist, token, message1);
                    }
                    catch (Exception ex)
                    {
                        return (false, false, null, ex.Message);
                    }
                }
               

                else
                {

                    return (false, false, null, "Invalid role specified.");
                }

            }
            catch (Exception ex)
            {
                return (false, false, null, ex.Message);
            }
        }
    }
}

