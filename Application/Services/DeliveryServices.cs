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
using onlineshopowner_api.Infrastructure.Models;
using StackExchange.Redis;
using System.Net.NetworkInformation;
using onlineshopowner_api.Application.Interfaces.Itoken;

namespace onlineshopowner_api.Application.Services
{
    public class DeliveryServices : IDeliveryServices
    {
        private IAuthHelper authHelper;
      
        private IUnityOfWork unityOfWork;
        private IUserContextService userContextService;
        private IGoogleMapService googleMapService;
        private IjwtTokenGenerator tokenGenerator;
        public DeliveryServices(IAuthHelper authHelper, IUnityOfWork unityOfWork,IUserContextService userContextService,IGoogleMapService googleMapService,IjwtTokenGenerator tokenGenerator)
        {
            this.authHelper = authHelper;
            this.unityOfWork = unityOfWork;
            this.userContextService = userContextService;
            this.googleMapService = googleMapService;
            this.tokenGenerator = tokenGenerator;
        }

        /*  public async Task<(bool IsSuccess)> GetDelivaryAndPostLocation()
          {

          }*/

        public async Task<(bool issuccess, bool isfound, string message)> AddPersonDelivery(DeliveryPersonDto deliveryPersonDto)
        {
            try
            {
                var person = new Domain.Entities.Person
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
                    DeliveryProvider = deliveryprovider,


                };
                var (successaddpersondelivery, isfoundpersondelivery, messageerr) = await unityOfWork.DelivaryRepository.AddDeliveryPerson(DeliveryPerson);
                if (!successaddpersondelivery)
                    return (false, false, messageerr);
                if (isfoundpersondelivery)
                    return (true, true, "we found the person delivery");
                return (true, false, "we add");

            }
            catch (Exception ex)
            {
                return(false,false,ex.Message+"gus");
            }



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

                var deliveryagent = new Domain.Entities.Delivery.DeliveryAgent
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
                    return (true, true, messagerr);
                return (true, false, messagerr);
            }catch(Exception ex)
            {
                return(false,false,ex.Message+"tato");
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
                if (isfound) { return (true, true, "is found later on"); }
                return(true,false,message);

            }
            catch (Exception ex) 
            {
                return(false,false,ex.Message);
            }
        }
        public async Task<(bool issucess, List<DeliveryPersonDto> deliverypersondtos, List<DeliveryAgentDto> deliveryAgentdtos, DeliveryShopDto deliveryShopdto ,RouteInfoDto routeInfoDto , string message)> getdeliverylocationclient(OrderLocationDto locationclientDto)
        {
            try
            {
                if (locationclientDto == null) return (false, null, null,null,null,"data is empty");

                int _userId;
                try
                {
                    _userId = userContextService.GetUserId();
                }
                catch (UnauthorizedAccessException ex)
                {
                    return (false, null,null,null,null, ex.Message);
                }
                // Check if this user is a valid shop owner
                var personResult = await unityOfWork.PersonRepository.GetPersonByPersonId(_userId);
                if (!personResult.IsSuccess || !personResult.IsFound)
                    return (false, null, null, null, null, personResult.Error);


                var resultcheckdbShopLocation = await unityOfWork.ShopRepository.GetShopLocationById(locationclientDto.shopid);
                if (resultcheckdbShopLocation.Value == (null,null)) return (false,null, null,  null, null,"errrrrrrrr");
                if (!resultcheckdbShopLocation.IsSuccess) return (false, null,null, null, null, resultcheckdbShopLocation.Error);
                if (!resultcheckdbShopLocation.IsFound) return (false, null,null, null, null, "shop is not found");
                var(shoplatitude,shoplangitude)=resultcheckdbShopLocation.Value;
                var routeinfodto = await googleMapService.GetRouteInfoAsync(shoplatitude, shoplangitude, locationclientDto.latitude, locationclientDto.longitude);
                //if (routeinfodto == null) return (false, null, null, null, null, "error in find the route");
                var resultdbGetDeliveries=await unityOfWork.DelivaryRepository.GetDeliveryOnLocation(shoplatitude,shoplangitude, locationclientDto.latitude,locationclientDto.longitude,locationclientDto.shopid);
                if (resultdbGetDeliveries == null) return (false, null, null, null, null, "emptyresult");
                if(resultdbGetDeliveries.IsSuccess == false) return (false,null,null,null,null, resultdbGetDeliveries.Error);
                var deliverypersonsdtos=new List<DeliveryPersonDto>();
                var deliveryagentdtos=new List<DeliveryAgentDto>();
               foreach(var deliveryperson in resultdbGetDeliveries.Value.deliverypersons)
                {
                    var deliveryprviderdto = new DeliveryProviderDto
                    {
                        delivery_id=deliveryperson.DeliveryProvider.Delivery_id,
                        active_bit=deliveryperson.DeliveryProvider.active_bit,
                        note_text=deliveryperson.DeliveryProvider.note_text,
                        provider_type=deliveryperson.DeliveryProvider.provider_type,
                        regionnames = deliveryperson.DeliveryProvider.regionname,
                    };
                    var persondto = new PersonDto
                    {
                        first_name=deliveryperson.Person.FirstName,
                        last_name=deliveryperson.Person.LastName,
                        email=deliveryperson.Person.Email,
                        phonenumber=deliveryperson.Person.PhoneNumber,
                    };
                    var deliverypersondto = new DeliveryPersonDto
                    {
                        deliveryProviderDto=deliveryprviderdto,
                        persondto=persondto,
                        
                    };
                    deliverypersonsdtos.Add(deliverypersondto);
                }
               foreach(var deliveryagent in resultdbGetDeliveries.Value.deliveryAgents)
                {
                    var deliveryprviderdto = new DeliveryProviderDto
                    {
                        delivery_id = deliveryagent.deliveryprovider.Delivery_id,
                        active_bit = deliveryagent.deliveryprovider.active_bit,
                        note_text = deliveryagent.deliveryprovider.note_text,
                        provider_type = deliveryagent.deliveryprovider.provider_type,
                        regionnames = deliveryagent.deliveryprovider.regionname,
                    };
                    var deliveryagentdto = new DeliveryAgentDto
                    {
                        deliveryproviderdto = deliveryprviderdto,
                        email=deliveryagent.email,
                        phone_number=deliveryagent.phone_number,
                        name=deliveryagent.name,

                    };
                    deliveryagentdtos.Add(deliveryagentdto);
                }
             var  deliveryshop=resultdbGetDeliveries.Value.deliveryShop;
                var deliveryshopdto = new DeliveryShopDto();
                if (deliveryshop != null) {
                    try
                    {


                        var deliveryproviderdto = new DeliveryProviderDto
                        {
                            active_bit = deliveryshop.deliveryProvider.active_bit,
                            delivery_id = deliveryshop.deliveryProvider.Delivery_id,
                            note_text = deliveryshop.deliveryProvider.note_text,
                            provider_type = deliveryshop.deliveryProvider.provider_type,
                            regionnames = deliveryshop.deliveryProvider.regionname,
                        };

                        deliveryshopdto.deliveryProviderDto = deliveryproviderdto;
                    }
                    catch (Exception ex) {

                        return (false, null, null, null, null, ex.Message + ex.StackTrace + ex.InnerException + ex.ToString() + "nout");
                    }
                    }
                return (true,deliverypersonsdtos, deliveryagentdtos, deliveryshopdto, routeinfodto,"is success");


            }
            catch(Exception ex)
            {
                return(false, null, null, null,null,ex.Message+ex.StackTrace+ex.InnerException+ex.ToString()+"jdjjjj");
            }
       
        }
        public async Task<(bool issuccess, string message)> LoginDeliveryAgent(LoginDeliveryDto logindeliverydto)
        {
            try {
                if (logindeliverydto == null)
                    return (false, "no login delivery dto");
                LoginDelivery logindelivery = new LoginDelivery();
                if (logindeliverydto.deliverytype != "person" & logindeliverydto.deliverytype != "shop" & logindeliverydto.deliverytype != "agent")
                {
                    return (false, "there no deliverytype");
                }
                logindelivery.deliverytype = logindeliverydto.deliverytype;
                if (logindeliverydto.email != null)
                {
                    logindelivery.email = logindeliverydto.email;
                }
                if (logindeliverydto.phonenumber != null)
                {
                    logindelivery.phonenumber = logindeliverydto.phonenumber;
                }
                logindelivery.password = HashingPassword.HashPassword(logindeliverydto.password);

                var (issuccess, deliveryid, message) = await unityOfWork.DelivaryRepository.LoginDeliveryAgentRep(logindelivery);
                if (!issuccess)
                    return (false, message);
                string token = tokenGenerator.GenerateToken(deliveryid, "delivery", 60);
                return (issuccess, token);
            }catch(Exception ex)
            {
                return (false, ex.Message+"mmmkmm");
            }
            }

    }
}