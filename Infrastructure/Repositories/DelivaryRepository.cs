using onlineshopowner_api.Application.Dtos.DeliveryDtos;
using onlineshopowner_api.Domain.Constant;
using onlineshopowner_api.Domain.Entities.Delivery;
using onlineshopowner_api.Domain.Interfaces.IRepository;
using onlineshopowner_api.Infrastructure.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.UI.WebControls;

namespace onlineshopowner_api.Infrastructure.Repositories
{
    public class DelivaryRepository:IDelivaryRepository
    {

        private string connectionstring { get; }

        public DelivaryRepository(string connectionstring)
        {
            this.connectionstring = connectionstring;
        }
     /*   public async Task<ResultCheckdb<>> GetDelivaryOnlocation(decimal ShopLatitude,decimal ShopLongitude,decimal OrderLatitude,decimal OrderLongitude)
        {

        }*/
        public async Task<(bool issuccess,bool isfound,string message)> AddDeliveryPerson(DeliveryPersons deliveryPerson, bool isexistperson = false)
        {
            try
            {
                DataTable regiontable= new DataTable();
                regiontable.Columns.Add("region_name",typeof(string));
                foreach(string region in deliveryPerson.deliveryprovider.regionname)
                {
                    regiontable.Rows.Add(region);
                }
                DataTable hourworkingdaytable=new DataTable();
                hourworkingdaytable.Columns.Add("weekday", typeof(string));
                hourworkingdaytable.Columns.Add("open_time",typeof(TimeSpan));
                hourworkingdaytable.Columns.Add("close_time", typeof(TimeSpan));
                foreach(var hourworkingday in deliveryPerson.deliveryprovider.DeliveryWorkigHours)
                {
                   hourworkingdaytable.Rows.Add(hourworkingday);
                }
                
                if (deliveryPerson == null) { return (false,false, "no dataa to add "); };
                using (SqlConnection connect = new SqlConnection(connectionstring))
                {
                    await connect.OpenAsync();
                    using (SqlCommand command = new SqlCommand("AddDeliveryperson", connect))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                       // command.Parameters.AddWithValue("@existperson", deliveryPerson);
                        command.Parameters.AddWithValue("@first_name", deliveryPerson.Person.FirstName);
                        command.Parameters.AddWithValue("@last_name",deliveryPerson.Person.LastName );
                        command.Parameters.AddWithValue("@provider_type",deliveryPerson.deliveryprovider.provider_type);
                        command.Parameters.AddWithValue("@email", deliveryPerson.Person.Email);
                        command.Parameters.AddWithValue("@passward",deliveryPerson.Person.Password );
                        command.Parameters.AddWithValue("@sex", deliveryPerson.Person.Sex);
                        command.Parameters.AddWithValue("@phonenumber", deliveryPerson.Person.PhoneNumber);
                        command.Parameters.AddWithValue("@note_text", deliveryPerson.deliveryprovider.note_text);
                        command.Parameters.AddWithValue("@active_bit", deliveryPerson.deliveryprovider.active_bit);
                       
                       
                        SqlParameter isFoundParam = new SqlParameter("@isfound", SqlDbType.Bit)
                        {
                            Direction = ParameterDirection.Output
                        };
                        command.Parameters.Add(isFoundParam);

                        SqlParameter regionParam = command.Parameters.AddWithValue("@regionnames", regiontable);
                        regionParam.SqlDbType = SqlDbType.Structured;
                        regionParam.TypeName = "dbo.regionnames"; // Make sure this matches the SQL type
                        SqlParameter hourworkdayparam = command.Parameters.AddWithValue("@hourworkday", hourworkingdaytable);
                        hourworkdayparam.SqlDbType = SqlDbType.Structured;
                        hourworkdayparam.TypeName = "dbo.Deliveryworkinghourtable";
                        await command.ExecuteNonQueryAsync();
                        connect.Close();
                        bool isFound = Convert.ToBoolean(isFoundParam.Value);
                        if(isFound)
                        {
                            return (true, true, "is exist ");
                        }

                        return (true, false,"success"); 

                    }
                }
            }
            catch (Exception ex) 
            {
                return(false,false,ex.Message);
            }

        }
        public async Task<(bool issuccess,bool isfound ,string message)> AddAgentDelivery(Domain.Entities.Delivery.DeliveryAgent deliveryAgent)
        {
            try
            {
                DataTable regiontable = new DataTable();
                regiontable.Columns.Add("region_name", typeof(string));
                foreach (string region in deliveryAgent.deliveryprovider.regionname)
                {
                    regiontable.Rows.Add(region);
                }
                DataTable hourworkingdaytable = new DataTable();
                hourworkingdaytable.Columns.Add("weekday", typeof(string));
                hourworkingdaytable.Columns.Add("open_time", typeof(TimeSpan));
                hourworkingdaytable.Columns.Add("close_time", typeof(TimeSpan));
                foreach (var hourworkingday in deliveryAgent.deliveryprovider.DeliveryWorkigHours)
                {
                    hourworkingdaytable.Rows.Add(hourworkingday);
                }

                if (deliveryAgent.deliveryprovider == null) 
                {
                    return (false, false, "no data to add to database");
                }

                using (SqlConnection connection = new SqlConnection(connectionstring))
                {
                    try
                    {
                        await connection.OpenAsync();
                        using (SqlCommand command = new SqlCommand("AddDeliveryAgent", connection))
                        {
                            command.Parameters.AddWithValue("@name", deliveryAgent.name);
                            command.Parameters.AddWithValue("@phone_number", deliveryAgent.phone_number);
                            command.Parameters.AddWithValue("@email", deliveryAgent.email);
                            command.Parameters.AddWithValue("@password", deliveryAgent.password);
                            command.Parameters.AddWithValue("@provider_type", deliveryAgent.deliveryprovider.provider_type);
                            command.Parameters.AddWithValue("@note_text", deliveryAgent.deliveryprovider.note_text);
                            command.Parameters.AddWithValue("@active_bit", deliveryAgent.deliveryprovider.active_bit);
                            SqlParameter hourworkdayparam = command.Parameters.AddWithValue("@workinghourtable", hourworkingdaytable);
                            hourworkdayparam.SqlDbType = SqlDbType.Structured;
                            hourworkdayparam.TypeName = "dbo.Deliveryworkinghourtable";
                            SqlParameter regionParam = command.Parameters.AddWithValue("@region", regiontable);
                            regionParam.SqlDbType = SqlDbType.Structured;
                            regionParam.TypeName = "dbo.regionnames";

                            SqlParameter isFoundParam = new SqlParameter("@isfound", SqlDbType.Bit)
                            {
                                Direction = ParameterDirection.Output
                            };
                            command.Parameters.Add(isFoundParam);

                            await command.ExecuteNonQueryAsync();
                            bool isFound = Convert.ToBoolean(isFoundParam.Value);
                            if (isFound)
                            {
                                return (true, true, "is exist ");

                            }
                            return (true, false, "good job man ");
                        }
                        
                    }
                    catch (Exception ex) 
                    {
                        return(false,false,ex.Message);
                    }

                }

            }
            catch (Exception ex) 
            {
                return(false,false,ex.Message);
            }

        }
        public async Task<(bool issucess,bool isfound,string message)>AddDeliveryShop(DeliveryShop deliveryShop)
        {
            try
            {
                DataTable regiontable = new DataTable();
                regiontable.Columns.Add("region_name", typeof(string));
                foreach (string region in deliveryShop.deliveryProvider.regionname)
                {
                    regiontable.Rows.Add(region);
                }
                DataTable hourworkingdaytable = new DataTable();
                hourworkingdaytable.Columns.Add("weekday", typeof(string));
                hourworkingdaytable.Columns.Add("open_time", typeof(TimeSpan));
                hourworkingdaytable.Columns.Add("close_time", typeof(TimeSpan));
                foreach (var hourworkingday in deliveryShop.deliveryProvider.DeliveryWorkigHours)
                {
                    hourworkingdaytable.Rows.Add(hourworkingday);
                }

                if (deliveryShop.deliveryProvider == null)
                {
                    return (false, false, "no data to add to database");
                }
                using (SqlConnection connect = new SqlConnection(connectionstring)) 
                {
                    await connect.OpenAsync();
                    using (SqlCommand command = new SqlCommand())
                    {
                        command.Parameters.AddWithValue("@shopid", deliveryShop.Shop_id);
                        command.Parameters.AddWithValue("@provider_type", deliveryShop.deliveryProvider.provider_type);
                        command.Parameters.AddWithValue("@note_text", deliveryShop.deliveryProvider.note_text);
                        command.Parameters.AddWithValue("@active_bit", deliveryShop.deliveryProvider.active_bit);
                        SqlParameter hourworkdayparam = command.Parameters.AddWithValue("@workinghourtable", hourworkingdaytable);
                        hourworkdayparam.SqlDbType = SqlDbType.Structured;
                        hourworkdayparam.TypeName = "dbo.Deliveryworkinghourtable";
                        SqlParameter regionParam = command.Parameters.AddWithValue("@region", regiontable);
                        regionParam.SqlDbType = SqlDbType.Structured;
                        regionParam.TypeName = "dbo.regionnames";

                        SqlParameter isFoundParam = new SqlParameter("@isfound", SqlDbType.Bit)
                        {
                            Direction = ParameterDirection.Output
                        };
                        command.Parameters.Add(isFoundParam);

                        await command.ExecuteNonQueryAsync();
                        bool isFound = Convert.ToBoolean(isFoundParam.Value);
                        if (isFound)
                        {
                            return (true, true, "is exist ");

                        }
                        return (true, false, "good job man ");
                    }
                }
            }
            catch (Exception ex)
            {
                return(false,false,ex.Message);
            }
        }
        public async Task<ResultCheckdb<List<GetDeliveryOnLocationDto>>> GetDeliveryOnLocation(decimal shoplatitude,decimal shoplongitude,decimal ClientLatitude,decimal ClientLongitude)
        {
            try
            {
                using(SqlConnection connect=new SqlConnection(connectionstring))
                {
                    using (SqlCommand command = new SqlCommand())
                    {
                        
                    }
                }
            }
            catch (Exception ex)
            {
                return new ResultCheckdb<List<GetDeliveryOnLocationDto>>
                {
                    IsFound = false,
                    IsSuccess = false,
                };
            }
        }
         
    }
}