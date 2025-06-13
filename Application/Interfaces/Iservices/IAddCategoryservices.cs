using onlineshopowner_api.Application.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace onlineshopowner_api.Application.Interfaces.Iservices
{
    public interface IAddCategoryservices
    {
        Task<(bool isAdmain, string message)> checkIfUserIsAdmin();
        Task<(bool issuces, string message)> addonecategory(CategoryDto dto);
        Task<(bool issuccess, string message)> addallcategory(List<CategoryDto> dtos);
    }
}
