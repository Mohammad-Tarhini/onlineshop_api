using onlineshopowner_api.Application.Dtos.PaymentAndOrder;
using onlineshopowner_api.Domain.Entities.PaymentAndOrder;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace onlineshopowner_api.Domain.Interfaces.IRepository
{
    public interface IPaymentAndOrderRepository
    {
        Task<int> RegisterOrder(clientOrder clientOrder, List<OrderDetail> orderDetails);
        Task<Domain.Entities.PaymentAndOrder.clientOrder> GetOrderByOrderId(int OrderId);
        Task updateStatusOnClientOrder(int orderId, string status);
        Task<List<Domain.Entities.PaymentAndOrder.clientOrder>> GetOrdersRequiredForsopOrDelivery(int ShopId = 0, int deliveryId = 0);
        Task<List<OrderDetail>> GetItemsOfOrder(int orderid);
         Task deliveryreciveorder(RecievefromShopToDeliveryDto recievefromShopToDelivery, int shopid);
        Task RecieveOrederFromDeliveryToClient(RecieveFromDeliveryToClientDto recievefromdeliverytoclient);
        //Task<(bool issucess, string message)> RegisterOrder(int clientid, decimal total_price, decimal DeliveryCost, decimal latitude, decimal longitude, int shopid, int deliveryid, DataTable producttable, string shopdeliverypin, string clientdeliverypin, string paymentmethode);
        //Task<ResultCheckdb<List<Domain.Entities.PaymentAndOrder.Order>>> GetNewOrder(int Shopid);
        //Task<ResultCheckdb<List<CartItem>>> GetItemsOfOrder(int orderid);
        //Task<ResultCheckdb<List<OrderForDelivery>>> GetOrdersForDelivery(int deliveryId);
        //Task<(bool issucess, bool isStartDelivery, string message)> deliveryreciveorder(RecievefromShopToDelivery recievefromShopToDelivery, int shopid);
        //Task<(bool issuccess, bool isfound, string message)> RecieveOrederFromDeliveryToClient(RecieveFromDeliveryToClient recievefromdeliverytoclient);
    }
}