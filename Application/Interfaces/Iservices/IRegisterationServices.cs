using onlineshopowner_api.Application.Dtos;
using onlineshopowner_api.Application.Dtos.DeliveryDtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace onlineshopowner_api.Application.Interfaces.Iservices
{
    public  interface IRegisterationServices
    {
        Task RegisterClient(RegistrationRequestDto dto);
        Task VerifyRegisteration(string token);
        Task RegisterShopOwnerorDelivery(RegistrationRequestDto dto);
        Task AddVerifiedShopowner(VerifyOtpDto verifyOtpDto);
        Task AddVerifiedDelivery(DeliveryProviderDto deliveryProviderDto, VerifyOtpDto verifyOtpDto);
    }
}
