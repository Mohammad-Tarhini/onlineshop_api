using onlineshopowner_api.Application.Dtos;
using onlineshopowner_api.Application.Dtos.Payment;
using onlineshopowner_api.Application.Dtos.PaymentAndOrder;
using onlineshopowner_api.Domain.Entities.PaymentAndOrder;
using onlineshopowner_api.Infrastructure.ExternalServices.Payment;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace onlineshopowner_api.Application.Interfaces.Iservices
{
    public interface IOrderServices
    {
        Task<(string sessionId, string checkoutUrl)> registerOrder(PayDto dto);
        Task HandlePaymentWebhookAsync(GatewayPayment gatwayPayment);
        Task<List<returnOrderForShopDto>> ReturnOrdersForShop();
        Task<List<returnOrderForDeliveryDto>> ReturnOrdersForDelivery();
        Task<List<returnItemOrder>> GetItemsOfOrder(int orderId);
        Task takeorderfromshoptodelivery(RecievefromShopToDeliveryDto recievefromShopToDeliverydto);
        Task RecieveOrderFromDeliveryTOClient(RecieveFromDeliveryToClientDto recievefromDeliveryToClient);
        
    }
}
