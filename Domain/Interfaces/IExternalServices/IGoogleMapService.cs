using onlineshopowner_api.Application.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace onlineshopowner_api.Domain.Interfaces.IExternalServices
{
    public interface IGoogleMapService
    {
        Task<RouteInfoDto> GetRouteInfoAsync(
            decimal originLat, decimal originLng,
            decimal destinationLat, decimal destinationLng);
    }
}
