using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Net.Http;
using Newtonsoft.Json;
using onlineshopowner_api.Infrastructure.ExternalServices.googlemap;
using onlineshopowner_api.Application.Dtos;
using onlineshopowner_api.Domain.Interfaces.IExternalServices;
using System.Configuration;

namespace onlineshopowner_api.Infrastructure.ExternalServices
{

    public class GoogleMapService : IGoogleMapService
    {
        private readonly string _apiKey = ConfigurationManager.AppSettings["GoogleApiKey"];

        public async Task<RouteInfoDto> GetRouteInfoAsync(
            decimal originLat, decimal originLng,
            decimal destinationLat, decimal destinationLng)
        {
            var origin = $"{originLat},{originLng}";
            var destination = $"{destinationLat},{destinationLng}";
            var url = $"https://maps.googleapis.com/maps/api/directions/json?" +
                      $"origin={origin}&destination={destination}&mode=driving&key={_apiKey}";

            using (HttpClient client = new HttpClient())
            {
                var json = await client.GetStringAsync(url);
                var response = JsonConvert.DeserializeObject<DirectionsResponse>(json);

                if (response?.status == "OK" && response.routes.Count > 0)
                {
                    var leg = response.routes[0].legs[0];
                    var polyline = response.routes[0].overview_polyline.points;

                    return new RouteInfoDto
                    {
                        Distance = leg.distance.text,
                        Duration = leg.duration.text,
                        EncodedPolyline = polyline
                    };
                }

                // Handle error or no route
                return null;
            }
        }
    }
    }