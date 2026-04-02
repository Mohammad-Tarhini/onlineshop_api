using onlineshopowner_api.Application.Dtos;
using onlineshopowner_api.Application.Dtos.DeliveryDtos;
using onlineshopowner_api.Application.Interfaces.Iservices;
using onlineshopowner_api.Application.Interfaces.Ivalidator;
using onlineshopowner_api.Application.Validatorandclean;
using onlineshopowner_api.Domain.Entities;
using onlineshopowner_api.Domain.Entities.Delivery;
using onlineshopowner_api.Domain.Interfaces.IRepository;
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
        public DeliveryServices(IAuthHelper authHelper, IUnityOfWork unityOfWork, IUserContextService userContextService, IGoogleMapService googleMapService, IjwtTokenGenerator tokenGenerator)
        {
            this.authHelper = authHelper;
            this.unityOfWork = unityOfWork;
            this.userContextService = userContextService;
            this.googleMapService = googleMapService;
            this.tokenGenerator = tokenGenerator;
        }

        public async Task<List<GetDeliveryOnLocationDto>>GetDeliveryAccordingToLocationAsync(OrderLocationDto locationDto)
        {
            if (locationDto == null)
                throw new ArgumentNullException(nameof(locationDto));

            var userId = userContextService.GetUserId();
            if (userId <= 0)
                throw new UnauthorizedAccessException("User not authenticated.");

            var shopLocation = await unityOfWork
                .ShopRepository
                .GetShopLocationById(locationDto.shopid)
                ?? throw new InvalidOperationException("Shop not found.");

            var routeInfo = await googleMapService
                .GetRouteInfoAsync(shopLocation.shopLatitude,
                                   shopLocation.shopLongitude,
                                   locationDto.latitude,
                                   locationDto.longitude)
                ?? throw new InvalidOperationException("Route calculation failed.");

            var deliveries = await unityOfWork
                .DelivaryRepository
                .GetDeliveryOnLocation(shopLocation.shopLatitude,
                                       shopLocation.shopLongitude,
                                       locationDto.latitude,
                                       locationDto.longitude);

            if (deliveries == null || !deliveries.Any())
                return new List<GetDeliveryOnLocationDto>();

            return deliveries.Select(d => new GetDeliveryOnLocationDto
            {
                Delivery_id = d.Delivery_id,
                name = $"{d.first_name} {d.last_name}",
                phonenumber = d.phonenumber,
                email = d.email,
                pricePerKm = d.pricePerKm,
                distance = routeInfo.Distance,
               // totalPrice = d.pricePerKm * routeInfo.Distance
            }).ToList();
        }
        //    public async Task<List<GetDeliveryOnLocationDto>> GetDelivaryAcordingForlocationService(OrderLocationDto locationclientDto)
        //    {

        //        if (locationclientDto == null)
        //            throw new Exception("no data at all");

        //        int userId;
        //        string userRole;

        //        userId = userContextService.GetUserId();
        //        userRole = userContextService.GetUserRole();
        //        if (userId == 0 || userId == null)
        //        {
        //            throw new Exception("invalid user id");
        //        }

        //        var shopLocation = await unityOfWork.ShopRepository.GetShopLocationById(locationclientDto.shopid);
        //        if (shopLocation == null)
        //        {
        //            throw new Exception("invalid shop id");
        //        }
        //        var (shopLatitude, shopLongitude) = shopLocation.Value;
        //        var routeinfodto = await googleMapService.GetRouteInfoAsync(shopLatitude, shopLongitude, locationclientDto.latitude, locationclientDto.longitude);

        //        if (routeinfodto == null)
        //            throw new Exception("error in find the route");

        //        var resultdbGetDeliveries = await unityOfWork.DelivaryRepository.GetDeliveryOnLocation(shopLatitude, shopLongitude, locationclientDto.latitude, locationclientDto.longitude);
        //        if (resultdbGetDeliveries == null)
        //            throw new Exception("error in find the delivery providers");
        //        var GetDeliveryDtos = new List<GetDeliveryOnLocationDto>();
        //        foreach (var item in resultdbGetDeliveries)
        //        {
        //            var deliveryDto = new GetDeliveryOnLocationDto
        //            {
        //               Delivery_id= item.Delivery_id,
        //                name = item.first_name+" "+item.last_name,
        //                phonenumber = item.phonenumber,
        //                email = item.email,
        //                pricePerKm = item.pricePerKm,
        //                distance = routeinfodto.Distance,

        //            };
        //            GetDeliveryDtos.Add(deliveryDto);
        //        }
        //        return GetDeliveryDtos;
        //    }
        //}
    }
}

               
      
    