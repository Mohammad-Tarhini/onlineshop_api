using onlineshopowner_api.Application.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace onlineshopowner_api.Application.Interfaces.Iservices
{
    public interface IOpenNewShopServices
    {
        Task<(bool IsSuccess, string message)> OpenShop(OpenNewShopDto dto);
    }
}