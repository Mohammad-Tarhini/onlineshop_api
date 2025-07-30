using onlineshopowner_api.Application.Dtos;
using onlineshopowner_api.Application.Dtos.DeliveryDtos;
using onlineshopowner_api.Application.Interfaces.Iservices;
using onlineshopowner_api.Application.Interfaces.Ivalidator;
using onlineshopowner_api.Application.Validatorandclean;
using onlineshopowner_api.Domain.Entities;
using onlineshopowner_api.Domain.Entities.Delivery;
using onlineshopowner_api.Domain.Interfaces.IRepository;
using onlineshopowner_api.Domain.Constant;
using onlineshopowner_api.Infrastructure.Repositories;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using onlineshopowner_api.Infrastructure.ExternalServices;
using onlineshopowner_api.Domain.Interfaces.IExternalServices;

namespace onlineshopowner_api.Application.Services
{
    public class DeliveryServices : IDeliveryServices
    {
        private IAuthHelper authHelper;
        private IpersonRepository personreposiroty;
        private IDelivaryRepository delivaryRepository;
        private IUnityOfWork unityOfWork;
        private IUserContextService userContextService;
        private IGoogleMapService googleMapService;
        public DeliveryServices(IAuthHelper authHelper, IUnityOfWork unityOfWork,IUserContextService userContextService,IGoogleMapService googleMapService)
        {
            this.authHelper = authHelper;
            this.unityOfWork = unityOfWork;
            this.userContextService = userContextService;
            this.googleMapService = googleMapService;
        }

        /*  public async Task<(bool IsSuccess)> GetDelivaryAndPostLocation()
          {

          }*/

        public async Task<(bool issuccess, bool isfound, string message)> AddPersonDelivery(DeliveryPersonDto deliveryPersonDto)
        {
            var person = new Person
            {
                FirstName = deliveryPersonDto.persondto.first_name,
                LastName = deliveryPersonDto.persondto.last_name,
                Sex = deliveryPersonDto.persondto.sex,
                Password = HashingPassword.HashPassword(deliveryPersonDto.persondto.password),
                Email = deliveryPersonDto.persondto.email,
                PhoneNumber = deliveryPersonDto.persondto.phonenumber,

            };
            var workinghours = new List<DeliveryWorkigHours>();
            foreach (var workhour in deliveryPersonDto.deliveryProviderDto.workHours)
            {
                var deliveryWorkinghour = new DeliveryWorkigHours
                {
                    Open_time = workhour.Open_time,
                    Close_time = workhour.Close_time,
                    WeekDay = workhour.WeekDay,
                };
                workinghours.Add(deliveryWorkinghour);
            }
            var deliveryprovider = new Domain.Entities.Delivery.DeliveryProvider
            {
                active_bit = deliveryPersonDto.deliveryProviderDto.active_bit,
                note_text = deliveryPersonDto.deliveryProviderDto.note_text,
                provider_type = deliveryPersonDto.deliveryProviderDto.provider_type,
                regionname = deliveryPersonDto.deliveryProviderDto.regionnames,
                DeliveryWorkigHours = workinghours,

            };
            var DeliveryPerson = new Domain.Entities.Delivery.DeliveryPersons
            {
                Person = person,
                deliveryprovider = deliveryprovider,


            };
            var (successaddpersondelivery, isfoundpersondelivery, messageerr) = await unityOfWork.DelivaryRepository.AddDeliveryPerson(DeliveryPerson);
            if (!successaddpersondelivery)
                return (false, false, messageerr);
            if (isfoundpersondelivery)
                return (true, true, "we found the person delivery");
            return (true, false, "we add");





        }
        public async Task<(bool issuccess, bool isfound, string message)> AddDeliveryAgent(DeliveryAgentDto deliveryAgentDto)
        {
            try {
                var workinghours = new List<DeliveryWorkigHours>();
                foreach (var workhour in deliveryAgentDto.deliveryproviderdto.workHours)
                {
                    var deliveryWorkinghour = new DeliveryWorkigHours
                    {
                        Open_time = workhour.Open_time,
                        Close_time = workhour.Close_time,
                        WeekDay = workhour.WeekDay,
                    };
                    workinghours.Add(deliveryWorkinghour);
                }



                var deliveryprovider = new Domain.Entities.Delivery.DeliveryProvider
                {
                    active_bit = deliveryAgentDto.deliveryproviderdto.active_bit,
                    note_text = deliveryAgentDto.deliveryproviderdto.note_text,
                    provider_type = deliveryAgentDto.deliveryproviderdto.provider_type,
                    DeliveryWorkigHours = workinghours,
                    regionname = deliveryAgentDto.deliveryproviderdto.regionnames,

                };

                var deliveryagent = new DeliveryAgent
                {
                    deliveryprovider = deliveryprovider,
                    email = deliveryAgentDto.email,
                    name = deliveryAgentDto.name,
                    password = HashingPassword.HashPassword(deliveryAgentDto.password),
                    phone_number = deliveryAgentDto.phone_number,
                };
                var (issuccessadddelivelyagent, isfoundagent, messagerr) = await unityOfWork.DelivaryRepository.AddAgentDelivery(deliveryagent);
                if (!issuccessadddelivelyagent)
                    return (false, false, messagerr);
                if (isfoundagent)
                    return (true, false, messagerr);
                return (true, true, messagerr);
            }catch(Exception ex)
            {
                return(false,false,ex.Message);
            }
        
        }  

        public async Task<(bool issuccess,bool isfound ,string message)> AddShopDelivery(DeliveryShopDto deliveryShopDto)
        {
            try
            {
                int _userId;
                try
                {
                    _userId = userContextService.GetUserId();
                }
                catch (UnauthorizedAccessException ex)
                {
                    return (false,false, ex.Message);
                }
                // Check if this user is a valid shop owner
                var personResult = await unityOfWork.PersonRepository.GetPersonByPersonId(_userId);
                if (!personResult.IsSuccess || !personResult.IsFound)
                    return (false,false, personResult.Error);

                var shopOwnerResult = await unityOfWork.PersonRepository.GetShopOwnerByPersonAsync(personResult.Value);
                if (!shopOwnerResult.IsSuccess || !shopOwnerResult.IsFound)
                    return (false,false, shopOwnerResult.Error);

                var shopOwner = shopOwnerResult.Value;

                // Check if the shop belongs to the shop owner
                var shopResult = await unityOfWork.ShopRepository.GetShopByShopOwner(shopOwner);
                if (!shopResult.IsSuccess || !shopResult.IsFound)
                    return (false,false, shopResult.Error);

                var shopid = shopResult.Value.shopid;



                var workinghours = new List<DeliveryWorkigHours>();
                foreach (var workhour in deliveryShopDto.deliveryProviderDto.workHours)
                {
                    var deliveryWorkinghour = new DeliveryWorkigHours
                    {
                        Open_time = workhour.Open_time,
                        Close_time = workhour.Close_time,
                        WeekDay = workhour.WeekDay,
                    };
                    workinghours.Add(deliveryWorkinghour);
                }



                var deliveryprovider = new Domain.Entities.Delivery.DeliveryProvider
                {
                    active_bit = deliveryShopDto.deliveryProviderDto.active_bit,
                    note_text = deliveryShopDto.deliveryProviderDto.note_text,
                    provider_type = deliveryShopDto.deliveryProviderDto.provider_type,
                    DeliveryWorkigHours = workinghours,
                    regionname = deliveryShopDto.deliveryProviderDto.regionnames,

                };

                var deliveryshop = new Domain.Entities.Delivery.DeliveryShop
                {
                    deliveryProvider = deliveryprovider,
                    Shop_id=shopid
                };
                var (issuccess,isfound,message)=await unityOfWork.DelivaryRepository.AddDeliveryShop(deliveryshop);
                if (!issuccess) { return (false, false, message); }
                if (isfound) { return (true, false, "is found later on"); }
                return(true,true,message);

            }
            catch (Exception ex) 
            {
                return(false,false,ex.Message);
            }
        }
        public async Task<(bool issucess, List<GetDeliveryOnLocationDto> deliveryOnLocationDtos,RouteInfoDto routeInfoDto , string message)> getdeliverylocationclient(OrderLocationDto locationclientDto)
        {
            try
            {
                if (locationclientDto == null) return (false, null, null,"data is empty");
                var resultcheckdbShopLocation = await unityOfWork.ShopRepository.GetShopLocationById(locationclientDto.shopid);
                if (resultcheckdbShopLocation == null) return (false,null, null, "errrrrrrrr");
                if (!resultcheckdbShopLocation.IsSuccess) return (false, null,null, resultcheckdbShopLocation.Error);
                if (!resultcheckdbShopLocation.IsFound) return (false, null,null, "shop is not found");
                var(shoplatitude,shoplangitude)=resultcheckdbShopLocation.Value;
                var routeinfodto = await googleMapService.GetRouteInfoAsync(shoplatitude, shoplangitude, locationclientDto.latitude, locationclientDto.longitude);
                var
             
            }catch(Exception ex)
            {
                return(false,null,null,ex.Message);
            }
       
        }
    }
}