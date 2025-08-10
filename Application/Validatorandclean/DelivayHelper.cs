using onlineshopowner_api.Application.Dtos;
using onlineshopowner_api.Domain.Interfaces.IExternalServices;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace onlineshopowner_api.Application.Validatorandclean
{
    public class DelivayHelper
    {
        private readonly IGoogleMapService googleMap;
        public DelivayHelper(IGoogleMapService googleMap) 
        {
            this.googleMap = googleMap;
        }
       

        public async Task<(bool IsSuccess, RouteInfoDto routeInfoDto,string message)> GetTheRouteInfo(decimal originLat, decimal originLng,
            decimal destinationLat, decimal destinationLng)
        {
            var routeInfoDto = await googleMap.GetRouteInfoAsync(originLat, originLng,
             destinationLat, destinationLng);
            if (routeInfoDto == null)
            {
                return (false, routeInfoDto,"the error in get route from google map");
            }
            else 
            { 
                return (true, routeInfoDto,"null");
            }
        }

        //public async Task<(bool issuccess, bool isfound, List<DeliveryGetDto>, string message)> GetAllDelivayOnlocation() { }
        //public async Task<(bool issuccess, bool isfound ,)>
            
    }
}