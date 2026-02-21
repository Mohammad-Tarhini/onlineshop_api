using Microsoft.Owin.Security;
using onlineshopowner_api.Application.Dtos.DeliveryDtos;
using onlineshopowner_api.Domain.Entities.Delivery;
using onlineshopowner_api.Domain.Interfaces.IRepository;
using onlineshopowner_api.Infrastructure.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.UI.WebControls;

namespace onlineshopowner_api.Infrastructure.Repositories
{
    public class DelivaryRepository : IDelivaryRepository
    {

        private string connectionstring { get; }
        private readonly online_shopEntities1 _dbContext;
        public DelivaryRepository(online_shopEntities1 dbContext)
        {
            this.connectionstring = ConfigurationManager.ConnectionStrings["online_shopAdo"].ConnectionString;
            _dbContext = dbContext;
        }
        //+++++++++++++++++++++++++++new+++++++++++++++++++++++++++++++++++++++++++++++++++++++++
        public async Task<int> AddDeliveryProvider(Domain.Entities.Delivery.DeliveryProvider deliveryProvider)
        {
            var deliveryProviderdb = new Models.DeliveryProvider
            {
                note_text = deliveryProvider.note_text,
                active_bit = deliveryProvider.active_bit,
                person_id = deliveryProvider.person_id,
                Provider_Type = deliveryProvider.provider_type,

            };
            _dbContext.DeliveryProviders.Add(deliveryProviderdb);
           await _dbContext.SaveChangesAsync();
            return deliveryProviderdb.delivery_Id;


        }
        public async Task AddDeliveryWorkingHour(Domain.Entities.Delivery.DeliveryWorkigHours deliveryWorkigHours)
        {
            var deliveryWorkingHourDB = new Models.DeliveryWorkingHour
            {
                close_time = deliveryWorkigHours.Close_time,
                open_time = deliveryWorkigHours.Open_time,
                weekday = deliveryWorkigHours.WeekDay,
                delivery_id = deliveryWorkigHours.DeliveryId,
            };
            _dbContext.DeliveryWorkingHours.Add(deliveryWorkingHourDB);
        }

        public async Task AddDeliveryRegion(Domain.Entities.Delivery.DeliveryRegion deliveryRegion)
        {
            var deliveryRegionDB = new Models.regiondelivery
            {
                delivery_id = deliveryRegion.DeliveryId,
                region_id = deliveryRegion.RegionId,

            };
            _dbContext.regiondeliveries.Add(deliveryRegionDB);

        }
        public async Task<int?> GetRegionIdByRegionName(string regionname)
        {
            var region = _dbContext.Regions.FirstOrDefault(r => r.Name == regionname);
            return region.Region_id;
        }




        public async Task<List<GetDelivery>> GetDeliveryOnLocation(decimal shoplatitude, decimal shoplongitude, decimal ClientLatitude, decimal ClientLongitude)
        {
            using (SqlConnection connect = new SqlConnection(connectionstring))
            {
                await connect.OpenAsync();
                using (SqlCommand command = new SqlCommand("GetDelivery", connect))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@shoplatitude", shoplatitude);
                    command.Parameters.AddWithValue("@shoplongitude", shoplongitude);
                    command.Parameters.AddWithValue("@clientlatitude", ClientLatitude);
                    command.Parameters.AddWithValue("@clientlongitude", ClientLongitude);

                    using (SqlDataReader reader = await command.ExecuteReaderAsync())
                    {
                        var deliveries = new List<GetDelivery>();



                        while (reader.Read())
                        {
                            var delivery = new Domain.Entities.Delivery.GetDelivery
                            {
                                Delivery_id = reader.GetInt32(reader.GetOrdinal("delivery_Id")),
                                provider_type = reader.GetString(reader.GetOrdinal("Provider_Type")),
                                note_text = reader.GetString(reader.GetOrdinal("note_box")),
                                first_name = reader.GetString(reader.GetOrdinal("first_name")),
                                last_name = reader.GetString(reader.GetOrdinal("last_name")),
                                email = reader.GetString(reader.GetOrdinal("email")),
                                phonenumber = reader.GetString(reader.GetOrdinal("phone_number")),
                                pricePerKm = reader.GetDecimal(reader.GetOrdinal("price_Per_m"))
                            };
                            deliveries.Add(delivery);
                        }
                        return deliveries;
                        connect.Close();
                    }
                }
            }
        }

        public async Task<Domain.Entities.Delivery.DeliveryProvider> GetDeliveryByDeliveryId(int deliveryid)
        {
            var deliverydb = _dbContext.DeliveryProviders.FirstOrDefault(dp => dp.delivery_Id == deliveryid);
            var delivey = new Domain.Entities.Delivery.DeliveryProvider
            {
                Delivery_id = deliverydb.delivery_Id,
                active_bit = deliverydb.active_bit.Value,

                note_text = deliverydb.note_text,
                person_id = deliverydb.person_id.Value,
                price_delivery_per_km = deliverydb.price_per_meter.Value,







            };
            return delivey;
        }
    }
}






    







        //+++++++++++++++++++++++++++++++++++++end  new  +++++++++++++++++++++++++++++++++++++++++++++++++++++++












