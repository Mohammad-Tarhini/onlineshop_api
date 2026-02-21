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

 

namespace onlineshopowner_api.Infrastructure.ExternalServices.googlemap
{

    public class GoogleMapService : IGoogleMapService
    {
        private readonly string _apiKey = ConfigurationManager.AppSettings["OpenRouteApiKey"];

        public async Task<RouteInfoDto> GetRouteInfoAsync(
     decimal originLat, decimal originLng,
     decimal destinationLat, decimal destinationLng)
        {
            var url = "https://api.openrouteservice.org/v2/directions/driving-car";

            // Build the request body
            var requestBody = new
            {
                coordinates = new[]
                {
            new[] { originLng, originLat },       // Note: ORS uses [lon, lat] order
            new[] { destinationLng, destinationLat }
        }
            };

            using (HttpClient client = new HttpClient())
            {
                // Add your ORS API key to headers
                client.DefaultRequestHeaders.Add("Authorization", _apiKey);
                client.DefaultRequestHeaders.Add("Accept", "application/json");

                var content = new StringContent(
                    JsonConvert.SerializeObject(requestBody),
                    System.Text.Encoding.UTF8,
                    "application/json"
                );

                var responseMessage = await client.PostAsync(url, content);
                var json = await responseMessage.Content.ReadAsStringAsync();

                var response = JsonConvert.DeserializeObject<OpenRouteResponse>(json);

                if (response?.routes != null && response.routes.Count > 0)
                {
                    var route = response.routes[0];

                    return new RouteInfoDto
                    {
                        Distance = route.summary.distance,          // in meters
                        Duration = (decimal)route.summary.duration,          // in seconds
                        EncodedPolyline = route.geometry            // ORS polyline
                    };
                }

                Console.WriteLine("OpenRouteService API failed or returned no routes.");
                return null;
            }
        }
    } 
    }