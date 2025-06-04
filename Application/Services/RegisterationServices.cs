using onlineshopowner_api.Application.Interfaces.Iservices;
using onlineshopowner_api.Application.Interfaces.Itoken;
using onlineshopowner_api.Infrastructure.Token;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using onlineshopowner_api.Domain.Constant;
using onlineshopowner_api.Application.Dtos;
using onlineshopowner_api.Application.Interfaces.Ivalidator;
using onlineshopowner_api.Infrastructure.Repositories;
using Antlr.Runtime;
using System.Threading.Tasks;

namespace onlineshopowner_api.Application.Services
{
    public class RegisterationServices : IRegisterationServices
    {
        private readonly IUnityOfWork _unitOfWork;
        private readonly IjwtTokenGenerator _tokenGenerator;
        private readonly Iregistrationhelper _registrationshelp;

        public RegisterationServices(
            IUnityOfWork unitOfWork,
            IjwtTokenGenerator tokenGenerator,
            Iregistrationhelper registrationhelp) 
        {
            _unitOfWork = unitOfWork;
            _tokenGenerator = tokenGenerator;
            _registrationshelp = registrationhelp;
        }

        public async Task<(bool IsSuccess,bool alreadyexist ,string Token, string Message)> RegisterAsync(RegisterationRequestDto dto)
        {


            try
            {
                var (issuccess, isfound, personToProcess, message) = await _registrationshelp.foundingperson(dto);


                if (!issuccess)
                {
                    return (false, false, null, message);
                }

                string roleToAssign = dto.role.ToLowerInvariant();


                if (dto.role == UserRoles.Client)
                {
                    try
                    {
                        var (IsSucces1, ClientAlreadyExist, message1) = await _registrationshelp.Registeronclient(isfound, personToProcess);
                        if (!issuccess)
                        {
                            return(false, false, null, message1);
                        }
                        var token = _tokenGenerator.GenerateToken(personToProcess.PersonId, roleToAssign, 60);



                        return (true, ClientAlreadyExist, token, message1);
                    }
                    catch (Exception ex)
                    {
                        return (false, false, null,ex.Message);
                    }

                }
                else if (roleToAssign == UserRoles.ShopOwner)
                {
                    try
                    {
                        var (IsSucces1, ShopOwnerAlreadyExist, message1) = await _registrationshelp.RegisterShopOwner(isfound, personToProcess);

                        await _unitOfWork.CommitAsync();

                        var token = _tokenGenerator.GenerateToken(personToProcess.PersonId, roleToAssign, 60);
                        return (IsSucces1, ShopOwnerAlreadyExist, token, message1);
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
