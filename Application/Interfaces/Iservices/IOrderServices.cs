using onlineshopowner_api.Application.Dtos;
using onlineshopowner_api.Application.Dtos.Payment;
using onlineshopowner_api.Application.Dtos.PaymentAndOrder;
using onlineshopowner_api.Domain.Entities.PaymentAndOrder;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace onlineshopowner_api.Application.Interfaces.Iservices
{
    public interface IOrderServices
    {
        Task<(bool issucess, List<CartItemCheckResponseDto> cartItemsResponse, decimal Totalprice, string message)> CheckCartItemAvailability(CartAvailabilityRequestDto dto);
        Task<(bool issuccess, string message)> PaymentAndRegisterOrder(PayDto paydto);
        Task<(bool issuccess, bool isfound, List<CartItem>, string message)> GetItemsfororder(int orderid);
        Task<(bool issuccess, bool isempty, List<Order>, string message)> GetOrdersForShop();
        Task<(bool issuccess, bool isfound, List<OrderForDelivery>, string message)> GetOrdersOfDelivery();
        Task<(bool issucces, string message)> takeorderfromshoptodelivery(RecievefromShopToDelivery recievefromShopToDeliverydto);
        Task<(bool issuccess, string message)> RecieveOrderFromDeliveryTOClient(RecieveFromDeliveryToClient recievefromDeliveryToClient);
    }
}
