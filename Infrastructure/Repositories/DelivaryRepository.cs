using onlineshopowner_api.Application.Dtos.DeliveryDtos;
using onlineshopowner_api.Domain.Constant;
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
    public class DelivaryRepository:IDelivaryRepository
    {

        private string connectionstring { get; }

        public DelivaryRepository()
        {
            this.connectionstring = ConfigurationManager.ConnectionStrings["online_shopAdo"].ConnectionString; 
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
                foreach(string region in deliveryPerson.DeliveryProvider.regionname)
                {
                    regiontable.Rows.Add(region);
                }
                DataTable hourworkingdaytable=new DataTable();
                hourworkingdaytable.Columns.Add("weekday", typeof(string));
                hourworkingdaytable.Columns.Add("open_time",typeof(TimeSpan));
                hourworkingdaytable.Columns.Add("close_time", typeof(TimeSpan));
                foreach(var hourworkingday in deliveryPerson.DeliveryProvider.DeliveryWorkigHours)
                {
                    hourworkingdaytable.Rows.Add(
                         hourworkingday.WeekDay,
                         hourworkingday.Open_time,
                         hourworkingday.Close_time
                     );
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
                        command.Parameters.AddWithValue("@provider_type",deliveryPerson.DeliveryProvider.provider_type);
                        command.Parameters.AddWithValue("@email", deliveryPerson.Person.Email);
                        command.Parameters.AddWithValue("@passward",deliveryPerson.Person.Password );
                        command.Parameters.AddWithValue("@sex", deliveryPerson.Person.Sex);
                        command.Parameters.AddWithValue("@phonenumber", deliveryPerson.Person.PhoneNumber);
                        command.Parameters.AddWithValue("@note_text", deliveryPerson.DeliveryProvider.note_text);
                        command.Parameters.AddWithValue("@active_bit", deliveryPerson.DeliveryProvider.active_bit);
                       
                       
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
                return(false,false,ex.Message+"lol");
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
                    hourworkingdaytable.Rows.Add(
                         hourworkingday.WeekDay,
                         hourworkingday.Open_time,
                         hourworkingday.Close_time
                     );
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
                            command.CommandType = CommandType.StoredProcedure;
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
                        return(false,false,ex.Message+"abbass");
                    }

                }

            }
            catch (Exception ex) 
            {
                return(false,false,ex.Message+"guys");
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
                    hourworkingdaytable.Rows.Add(
                         hourworkingday.WeekDay,
                         hourworkingday.Open_time,
                         hourworkingday.Close_time
                     );
                }

                if (deliveryShop.deliveryProvider == null)
                {
                    return (false, false, "no data to add to database");
                }
                using (SqlConnection connect = new SqlConnection(connectionstring)) 
                {
                    await connect.OpenAsync();
                    using (SqlCommand command = new SqlCommand("AddDeliveryShop",connect))
                    {
                        command.CommandType = CommandType.StoredProcedure;

                        command.Parameters.AddWithValue("@shopid", deliveryShop.Shop_id);
                        command.Parameters.AddWithValue("@provider_type", deliveryShop.deliveryProvider.provider_type);
                        command.Parameters.AddWithValue("@note_text", deliveryShop.deliveryProvider.note_text);
                        command.Parameters.AddWithValue("@active_bit", deliveryShop.deliveryProvider.active_bit);
                        SqlParameter hourworkdayparam = command.Parameters.AddWithValue("@hourworkday", hourworkingdaytable);
                        hourworkdayparam.SqlDbType = SqlDbType.Structured;
                        hourworkdayparam.TypeName = "dbo.Deliveryworkinghourtable";
                        SqlParameter regionParam = command.Parameters.AddWithValue("@regionnames", regiontable);
                        regionParam.SqlDbType = SqlDbType.Structured;
                        regionParam.TypeName = "dbo.regionnames";

                        SqlParameter isFoundParam = new SqlParameter("@isfound", SqlDbType.Bit)
                        {
                            Direction = ParameterDirection.Output
                        };
                        command.Parameters.Add(isFoundParam);

                        await command.ExecuteNonQueryAsync();
                        bool isFound = Convert.ToBoolean(isFoundParam.Value);
                        connect.Close();
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
                return(false,false,ex.Message+"heloooo");
            }
        }
        public async Task<ResultCheckdb<(List<DeliveryPersons> deliverypersons,List<Domain.Entities.Delivery.DeliveryAgent> deliveryAgents,DeliveryShop deliveryShop) >> GetDeliveryOnLocation(decimal shoplatitude,decimal shoplongitude,decimal ClientLatitude,decimal ClientLongitude ,int shopid)
        {
            try
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
                        command.Parameters.AddWithValue("@shopid", shopid);
                        using (SqlDataReader reader = await command.ExecuteReaderAsync())
                        {
                            var deliverypersons = new List<DeliveryPersons>();
                            var deliveryagents = new List<Domain.Entities.Delivery.DeliveryAgent>();



                            while (reader.Read())
                            {
                                var deliveryProvider = new Domain.Entities.Delivery.DeliveryProvider
                                {
                                    Delivery_id = reader.GetInt32(reader.GetOrdinal("delivery_Id")),
                                    provider_type = reader.GetString(reader.GetOrdinal("Provider_Type")),
                                    note_text = reader.GetString(reader.GetOrdinal("note_box")),
                                    active_bit = reader.GetBoolean(reader.GetOrdinal("active_bit")),
                                    Create_at = reader.GetDateTime(reader.GetOrdinal("created_at")),
                                };
                                var person = new Domain.Entities.Person
                                {
                                    FirstName = reader.GetString(reader.GetOrdinal("first_name")),
                                    LastName = reader.GetString(reader.GetOrdinal("last_name")),
                                    Email = reader.GetString(reader.GetOrdinal("email")),
                                    PhoneNumber = reader.GetString(reader.GetOrdinal("phone_number")),
                                };
                                var deliveryPerson = new Domain.Entities.Delivery.DeliveryPersons
                                {
                                    DeliveryProvider = deliveryProvider,
                                    Person = person,
                                };
                                deliverypersons.Add(deliveryPerson);
                            }
                            if (reader.NextResult())
                            {
                                while (reader.Read())
                                {
                                    var deliveryprovider = new Domain.Entities.Delivery.DeliveryProvider
                                    {
                                        Delivery_id = reader.GetInt32(reader.GetOrdinal("delivery_Id")),
                                        provider_type = reader.GetString(reader.GetOrdinal("Provider_Type")),
                                        note_text = reader.GetString(reader.GetOrdinal("note_text")),
                                        active_bit = reader.GetBoolean(reader.GetOrdinal("active_bit")),
                                        Create_at = reader.GetDateTime(reader.GetOrdinal("created_at")),

                                    };
                                    var deliveryagent = new Domain.Entities.Delivery.DeliveryAgent
                                    {
                                        name = reader.GetString(reader.GetOrdinal("name")),
                                        email = reader.GetString(reader.GetOrdinal("email")),
                                        phone_number = reader.GetString(reader.GetOrdinal("phone_number")),
                                        deliveryprovider = deliveryprovider,
                                    };
                                    deliveryagents.Add(deliveryagent);

                                }


                            }

                            DeliveryShop deliveryshop = null; // Declare outside the if blocks

                            if (reader.NextResult())
                            {
                                if (reader.Read())
                                {
                                    deliveryshop = new Domain.Entities.Delivery.DeliveryShop();
                                    var deliveryprovider = new Domain.Entities.Delivery.DeliveryProvider
                                    {
                                        Delivery_id = reader.GetInt32(reader.GetOrdinal("delivery_Id")),
                                        provider_type = reader.GetString(reader.GetOrdinal("Provider_Type")),
                                        note_text = reader.GetString(reader.GetOrdinal("note_text")),
                                        active_bit = reader.GetBoolean(reader.GetOrdinal("active_bit")),
                                        Create_at = reader.GetDateTime(reader.GetOrdinal("created_at")),
                                    };
                                    deliveryshop.deliveryProvider = deliveryprovider;
                                    deliveryshop.Shop_id = reader.GetInt32(reader.GetOrdinal("shop_id"));
                                }
                            }

                            // Then return it with the actual value
                            return new ResultCheckdb<(List<DeliveryPersons>, List<Domain.Entities.Delivery.DeliveryAgent>, DeliveryShop)>
                            {
                                IsSuccess = true,
                                Value = (deliverypersons, deliveryagents, deliveryshop)
                            };
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                return new ResultCheckdb<(List<DeliveryPersons> deliverypersons, List<Domain.Entities.Delivery.DeliveryAgent> deliveryAgents, DeliveryShop deliveryShop)>
                {
                    IsFound = false,
                    IsSuccess = false,
                    Error = ex.Message + ex.StackTrace

                };
            }
        }

        public async Task<ResultCheckdb<(string phonenumber, string email, string deliverytype,string name)>> GetPhoneAndEmailForDelivery(int deliveryproviderid)
        {
            try
            {
                using (SqlConnection connect = new SqlConnection(connectionstring)) 
                {
                    await connect.OpenAsync();
                    using (SqlCommand cmd = new SqlCommand("GetPhoneAndEmailDelivery", connect))
                    {

                        cmd.Parameters.AddWithValue("@deliveryproviderid", deliveryproviderid);

                        SqlParameter phonenumberparam = new SqlParameter("@phonenumber", SqlDbType.VarChar)
                        {
                            Direction = ParameterDirection.Output
                        };
                        cmd.Parameters.Add(phonenumberparam);

                        SqlParameter emailparam = new SqlParameter("@email", SqlDbType.VarChar)
                        {
                            Direction = ParameterDirection.Output,
                        };
                        cmd.Parameters.Add(emailparam);
                        SqlParameter deliverytypeparam = new SqlParameter("@providertype", SqlDbType.VarChar)
                        {
                            Direction = ParameterDirection.Output,
                        };
                        cmd.Parameters.Add(deliverytypeparam);
                        SqlParameter deliverynameparam = new SqlParameter("@deliveryname", SqlDbType.VarChar)
                        {
                            Direction = ParameterDirection.Output,
                        };
                        cmd.Parameters.Add(deliverynameparam);
                        var deliverytype=deliverytypeparam.Value.ToString();
                        var email = emailparam.Value.ToString();
                        var phonenumber = phonenumberparam.Value.ToString();
                        var deliveryname=deliverynameparam.Value.ToString();
                        if (email == null && phonenumber == null) 
                        {return(new ResultCheckdb<(string phonenumber, string email, string deliverytype,string name)> { IsFound=false, IsSuccess=true });
                        }

                        return (new ResultCheckdb<(string phonenumber, string email, string deliverytype,string name)>
                        {
                            Value = (phonenumber, email,deliverytype,deliveryname),
                            IsSuccess = true,
                            IsFound = true
                        });


                    }    
                }
            }
            catch (Exception ex)
            {
                return new ResultCheckdb<(string phonenumber, string email, string deliverytype,string name)>
                {
                    IsSuccess=false,
                };
            }
        }

        public async Task<(bool issucces,int deliveryid,string message)>LoginDeliveryAgentRep(LoginDelivery loginDelivery)
        {
            if (loginDelivery == null) return (false, 0,"weewhsihuih");
            try
            {
               
                using (SqlConnection connect = new SqlConnection(connectionstring))
                {
                    await connect.OpenAsync();
                    using (SqlCommand cmd = new SqlCommand("checkDelivery", connect))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;

                 
                        cmd.Parameters.AddWithValue("@password", loginDelivery.password);
                        cmd.Parameters.AddWithValue("@email",loginDelivery.email);
                        cmd.Parameters.AddWithValue("@phonenumber",loginDelivery.phonenumber);
                        cmd.Parameters.AddWithValue("@deliverytype",loginDelivery.deliverytype);

                        SqlParameter iscorrectparam = new SqlParameter("@iscorrect", SqlDbType.Bit)
                        {
                            Direction = ParameterDirection.Output,
                        };
                        cmd.Parameters.Add(iscorrectparam);
                        SqlParameter deliveryidparam = new SqlParameter("@deliveryid", SqlDbType.Int)
                        {
                            Direction = ParameterDirection.Output,
                        };
                        cmd.Parameters.Add(deliveryidparam);
                        await cmd.ExecuteNonQueryAsync();
                        connect.Close();
                        var iscorrect = iscorrectparam.Value != DBNull.Value && Convert.ToBoolean(iscorrectparam.Value);
                        int deliveryid = deliveryidparam.Value != DBNull.Value ? Convert.ToInt32(deliveryidparam.Value) : 0;

                        if (iscorrect)
                            return (true,deliveryid, "the delivery is not exist");
                        else return (false,0, "sorry this ");

                    }
                }
            }
            catch (Exception ex) 
            {
                return(false,0,ex.Message+"dkdk");
            }

        }

         
    }
}
