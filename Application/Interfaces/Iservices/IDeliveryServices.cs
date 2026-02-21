using onlineshopowner_api.Application.Dtos;
using onlineshopowner_api.Application.Dtos.DeliveryDtos;
using onlineshopowner_api.Domain.Entities.Delivery;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace onlineshopowner_api.Application.Interfaces.Iservices
{
    public interface IDeliveryServices
    {
        Task<List<GetDeliveryOnLocationDto>> GetDelivaryAcordingForlocationService(OrderLocationDto locationclientDto);
        //Task<(bool issuccess, bool isfound, string message)> AddPersonDelivery(DeliveryRegisterationRequestDto deliveryPersonDto);
        //Task<(bool issuccess, bool isfound, string message)> AddDeliveryAgent(DeliveryAgentDto deliveryAgentDto);
        //Task<(bool issuccess, bool isfound, string message)> AddShopDelivery(DeliveryShopDto deliveryShopDto);
        //Task<(bool issucess, List<DeliveryRegisterationRequestDto> deliverypersondtos, List<DeliveryAgentDto> deliveryAgentdtos, DeliveryShopDto deliveryShopdto, RouteInfoDto routeInfoDto, string message)> getdeliverylocationclient(OrderLocationDto locationclientDto);
        //Task<(bool issuccess, string message)> LoginDeliveryAgent(LoginDeliveryDto logindeliverydto);
    }
}