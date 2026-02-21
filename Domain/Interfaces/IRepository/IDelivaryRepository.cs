using onlineshopowner_api.Application.Dtos.DeliveryDtos;
using onlineshopowner_api.Domain.Constant;
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
        //+++++++++++++++++++++++++++++new++++++++++++++++++++++++++++++++++
        Task<int> AddDeliveryProvider(Domain.Entities.Delivery.DeliveryProvider deliveryProvider);
        Task AddDeliveryWorkingHour(Domain.Entities.Delivery.DeliveryWorkigHours deliveryWorkigHours);

        Task AddDeliveryRegion(Domain.Entities.Delivery.DeliveryRegion deliveryRegion);
        Task<int?> GetRegionIdByRegionName(string regionname);

        Task<List<GetDelivery>> GetDeliveryOnLocation(decimal shoplatitude, decimal shoplongitude, decimal ClientLatitude, decimal ClientLongitude);

        Task<Domain.Entities.Delivery.DeliveryProvider> GetDeliveryByDeliveryId(int deliveryid);


        //+++++++++++++++++++++++++++++old++++++++++++++++++++++++++++++++++
        //Task<(bool issuccess, bool isfound, string message)> AddDeliveryPerson(DeliveryPersons deliveryPerson, bool isexistperson = false);
        //Task<(bool issuccess, bool isfound, string message)> AddAgentDelivery(DeliveryAgent deliveryAgent);
        //Task<(bool issucess, bool isfound, string message)> AddDeliveryShop(DeliveryShop deliveryShop);
        //Task<ResultCheckdb<(List<DeliveryPersons> deliverypersons, List<Domain.Entities.Delivery.DeliveryAgent> deliveryAgents, DeliveryShop deliveryShop)>> GetDeliveryOnLocation(decimal shoplatitude, decimal shoplongitude, decimal ClientLatitude, decimal ClientLongitude, int shopid);
        //Task<ResultCheckdb<(string phonenumber, string email, string deliverytype, string name)>> GetPhoneAndEmailForDelivery(int deliveryproviderid);
        //Task<(bool issucces, int deliveryid, string message)> LoginDeliveryRep(LoginDelivery loginDelivery);
    }
}
