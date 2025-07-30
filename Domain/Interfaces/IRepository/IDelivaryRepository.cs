using onlineshopowner_api.Application.Dtos.DeliveryDtos;
using onlineshopowner_api.Domain.Entities.Delivery;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace onlineshopowner_api.Domain.Interfaces.IRepository
{
    public interface IDelivaryRepository
    {
        Task<(bool issuccess, bool isfound, string message)> AddDeliveryPerson(DeliveryPersons deliveryPerson, bool isexistperson = false);
        Task<(bool issuccess, bool isfound, string message)> AddAgentDelivery(DeliveryAgent deliveryAgent);
        Task<(bool issucess, bool isfound, string message)> AddDeliveryShop(DeliveryShop deliveryShop);
    }
}
