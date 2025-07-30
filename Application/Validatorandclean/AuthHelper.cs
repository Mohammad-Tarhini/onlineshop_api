using onlineshopowner_api.Application.Dtos;
using onlineshopowner_api.Application.Interfaces.Iservices;
using onlineshopowner_api.Application.Interfaces.Ivalidator;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace onlineshopowner_api.Application.Validatorandclean
{
    public class AuthHelper:IAuthHelper
    {
        private readonly IUnityOfWork _unitOfWork;
        public AuthHelper(IUnityOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork; 
        }
        public async Task<(bool IsSuccess, bool isfound, Domain.Entities.Person person, string message)> foundingperson(RegisterationRequestDto dto)
        {
            this.ValidateInput(dto);

            if (dto.personDto.email != null)
            {
                var theexistperson1 = await _unitOfWork.PersonRepository.GetPersonByEmailAsync(dto.personDto.email);
                if (theexistperson1.IsSuccess == false)
                {
                    return (false, false, null, theexistperson1.Error);
                }
                if (theexistperson1.IsSuccess == true)
                {
                    if (theexistperson1.IsFound == true)
                    {
                        return (true, true, theexistperson1.Value, theexistperson1.Error);
                    }

                }
            }
            if (dto.personDto.phonenumber != null)
            {
                var theexistperson2 = await _unitOfWork.PersonRepository.GetPersonByPhoneNumberAsync(dto.personDto.phonenumber);
                if (theexistperson2.IsSuccess == false)
                {
                    return (false, false, null, theexistperson2.Error);

                }
                else
                {
                    if (theexistperson2.IsFound)
                    {
                        return (true, true, theexistperson2.Value, theexistperson2.Error);
                    }
                }
            }

            var theexistperson3 = await _unitOfWork.PersonRepository.GetPersonByCredentialsAsync(dto.personDto.first_name, dto.personDto.last_name);
            if (theexistperson3.IsSuccess == false)
            {
                return (false, false, null, theexistperson3.Error);
            }
            if (theexistperson3.IsSuccess == true)
            {
                if (theexistperson3.IsFound == true)
                {
                    return (true, true, theexistperson3.Value, theexistperson3.Error);
                }
            }
            if (dto.role != "delivery" || dto.role != "delivary")
            {

                var personToProcess = new Domain.Entities.Person(
           firstname: dto.personDto.first_name, lastname: dto.personDto.last_name, email: dto.personDto.email, sex: dto.personDto.sex,
            phonenumber: dto.personDto.phonenumber, password: HashingPassword.HashPassword(dto.personDto.password));
                try
                {
                    await _unitOfWork.PersonRepository.AddPersonAsync(personToProcess);

                }
                catch (Exception ex) { return (false, false, null, "error in RSH M1" + ex.Message); };

                return (true, false, personToProcess, theexistperson3.Error);

            }
            return(true, false, null, "the delivery person is not found ");

        }
        public async Task<(bool IsSuccess, bool arleadyExistClient, string message)> Registeronclient(bool arleadexistperson, Domain.Entities.Person personToProcess)
        {

            try
            {

                if (arleadexistperson)
                {

                    var client = await _unitOfWork.PersonRepository.GetClientByPersonAsync(personToProcess);
                    if (!client.IsSuccess)
                    {//if not arleady exist person i should delete person data
                        return (false, false, client.Error);
                    }
                    if (client.IsFound)
                    {
                        return (true, true, client.Error);
                    }
                }
                try
                {
                    await _unitOfWork.PersonRepository.AssignClientRoleToPersonAsync(personToProcess);
                    await _unitOfWork.CommitAsync();

                    return (true, false, "New client registered successfully.");
                }
                catch (Exception ex)
                {   
                    return (false, false, "error RSH M2 2" + ex.Message);
                }
            }
            catch (Exception ex)
            {
                return (false, false, ex.Message);
            }
        }
        public async Task<(bool IsSuccess, bool arleadyExistShopOwner, string message)> RegisterShopOwner(bool arleadexistperson, Domain.Entities.Person personToProcess)
        {
            try
            {

                if (arleadexistperson)
                {

                    var shopowner = await _unitOfWork.PersonRepository.GetShopOwnerByPersonAsync(personToProcess);
                    if (!shopowner.IsSuccess)
                    {
                        return (false, false, shopowner.Error);
                    }
                    if (shopowner.IsFound)
                    {
                        return (true, true, shopowner.Error);
                    }
                }
                try
                {
                    await _unitOfWork.PersonRepository.AssignShopOwnerRoleToPersonAsync(personToProcess);
                    await _unitOfWork.CommitAsync();

                    return (true, false, "New shopowner  registered successfully.");
                }
                catch (Exception ex)
                {
                    return (false, false, "error RSH M2 2" + ex.Message);
                }
            }
            catch (Exception ex)
            {
                return (false, false, ex.Message);
            }

        }

        public async Task<(bool IsSuccess, bool arleadyExistadmin, string message)> Registeradmin(bool arleadexistperson, Domain.Entities.Person personToProcess)
        {

            try
            {

                if (arleadexistperson)
                {

                    var admain = await _unitOfWork.PersonRepository.GetAdminByPersonAsync(personToProcess);
                    if (!admain.IsSuccess)
                    {
                        return (false, false, admain.Error);
                    }
                    if (admain.IsFound)
                    {
                        return (true, true, admain.Error);
                    }
                }
                try
                {
                    await _unitOfWork.PersonRepository.AssignAdmintRoleToPersonAsync(personToProcess);
                    await _unitOfWork.CommitAsync();

                    return (true, false, "New admin registered successfully.");
                }
                catch (Exception ex)
                {
                    return (false, false, "error RSH M2 2" + ex.Message);
                }
            }
            catch (Exception ex)
            {
                return (false, false, ex.Message);
            }
        }

        public void ValidateInput(RegisterationRequestDto dto)
        {

            dto.CleanStrings();
            if (string.IsNullOrWhiteSpace(dto.role))
                throw new ArgumentNullException(nameof(dto.role), "role can not be null");
            if (dto.role != "client" && dto.role != "shopowner" && dto.role != "admin" && (dto.role != "delivary" || dto.role !="delivery"))
            {
                throw new ArgumentException(nameof(dto.role), "role should be client or shopowner");
            }

        }

        public async Task<(bool isfound, Domain.Entities.Person person, string message)> CheckExistPersonByEmail(LoginRequestDto dto)
        {
            Domain.Entities.Person thePerson;
            try
            {
                var thepersonE = await _unitOfWork.PersonRepository.GetPersonByEmailAsync(dto.email);
                if (!thepersonE.IsSuccess)
                {
                    return (false, null, "try enter by other way ");
                }

                thePerson = thepersonE.Value;

                if (thePerson.FirstName != dto.first_name || thePerson.LastName != dto.last_name)
                {
                    return (false, null, "the email or name are not related");
                }
                if (dto.phonenumber != null)
                {
                    if (dto.phonenumber != thePerson.PhoneNumber)
                    {
                        return (false, null, "the email or phone number are wrong ");
                    }
                }

                return (true, thePerson, "is  the person is found by email");
            }
            catch (Exception ex) { return (false, null, ex.Message); }
        }
        public async Task<(bool isfound, Domain.Entities.Person person, string message)> CheckExistPersonByPhoneNumber(LoginRequestDto dto)
        {
            Domain.Entities.Person thePerson;
            try
            {
                var thepesonPh = await _unitOfWork.PersonRepository.GetPersonByPhoneNumberAsync(dto.phonenumber);
                thePerson = thepesonPh.Value;
                if (!thepesonPh.IsSuccess)
                {
                    return (false, null, thepesonPh.Error);
                }
                if (!thepesonPh.IsFound)
                {
                    return (false, null, "the person is not found");
                }
                if (thePerson.FirstName != dto.first_name || thePerson.LastName != dto.last_name)
                {
                    return (false, null, "the phone number or name are not related");
                }

                if (dto.email != null)
                {
                    if (thePerson.PhoneNumber != dto.phonenumber)
                    {
                        return (false, null, "the email or  phone number are you enter are wrong");
                    }
                }
                return (true, thePerson, "the person exist");
            }
            catch (Exception ex) { return (false, null, ex.Message); }
        }




    }

}
 
