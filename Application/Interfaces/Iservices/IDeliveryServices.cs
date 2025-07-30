using onlineshopowner_api.Application.Dtos.DeliveryDtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace onlineshopowner_api.Application.Interfaces.Iservices
{
    public interface IDeliveryServices
    {
        Task<(bool issuccess, bool isfound, string message)> AddPersonDelivery(DeliveryPersonDto deliveryPersonDto);
        Task<(bool issuccess, bool isfound, string message)> AddDeliveryAgent(DeliveryAgentDto deliveryAgentDto);
        Task<(bool issuccess, bool isfound, string message)> AddShopDelivery(DeliveryShopDto deliveryShopDto);
    }
}