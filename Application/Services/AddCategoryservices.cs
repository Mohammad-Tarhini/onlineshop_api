using onlineshopowner_api.Application.Dtos;
using onlineshopowner_api.Application.Interfaces.Iservices;
using onlineshopowner_api.Domain.Interfaces.IExternalServices;
using onlineshopowner_api.Domain.Constant;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Policy;
using System.Threading.Tasks;
using System.Web;

namespace onlineshopowner_api.Application.Services
{
    public class AddCategoryservices:IAddCategoryservices
    {
        private readonly IUnityOfWork _unitOfWork;
        private readonly IUserContextService _usercontext;
       
        private int _userId;
        private string _role;

        public AddCategoryservices(IUnityOfWork unityOfWork, IUserContextService usercontextservice)
        {
            _unitOfWork = unityOfWork;
            _usercontext = usercontextservice;


        }

        public async Task<(bool isAdmain, string message)> checkIfUserIsAdmin()
        {
            try
            {
                _userId = _usercontext.GetUserId();
                _role=_usercontext.GetUserRole();
                if (_role != "admin") return (false, " ");

            }
            catch (Exception ex)
            {
                return (false, ex.Message);
            }

            var resultcheckadmain = await _unitOfWork.PersonRepository.checkAdmainbypersonid(_userId);
            if (resultcheckadmain == null)
            {
                return (false, "the error when check person repository");
            }
            if (!resultcheckadmain.IsSuccess) { return (false, resultcheckadmain.Error); }
            if (resultcheckadmain.IsFound) { return (false, resultcheckadmain.Error); }

            return (true, null);



        }
        public async Task<(bool issuces, string message)> addonecategory(CategoryDto dto)
        {
            try
            {
                var resultcheckcategory = await _unitOfWork.CategoryRepository.checkIfCategoryExist(dto.name);
                if (resultcheckcategory == null) { return (false, "error in checking category exist"); }
                if (!resultcheckcategory.IsSuccess) { return (false, resultcheckcategory.Error); }
                if (resultcheckcategory.IsFound) { return (false, resultcheckcategory.Error); }

            }
            catch (Exception ex) { return (false, ex.Message); }
            try
            {
                var resultaddcaregory = await _unitOfWork.CategoryRepository.Addcategory(dto.name);  
                if (resultaddcaregory == "success") { return (true, "congrute"); }
                else return (false, resultaddcaregory);
            }
            catch (Exception ex)
            {
                return (false, ex.Message);
            }
        }

        public async Task<(bool issuccess, string message)> addallcategory(List<CategoryDto> dtos)
        {
            foreach (var dto in dtos)
            {
                try
                {
                 var (addingcategorysuccess,message)=  await this.addonecategory(dto);
                    if(!addingcategorysuccess) return (false, message+": "+dto);
                    

                }
                catch (Exception ex)
                {
                    return(false, ex.Message);
                }
                
            }
            return (true, "all category are add");
        }


    }
}