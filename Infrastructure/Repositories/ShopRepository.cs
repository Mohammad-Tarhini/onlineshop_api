using onlineshopowner_api.Application.Dtos;
using onlineshopowner_api.Domain.Constant;
using onlineshopowner_api.Domain.Entities;
using onlineshopowner_api.Domain.Interfaces;
using onlineshopowner_api.Domain.Interfaces.IRepository;
using onlineshopowner_api.Infrastructure.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http.Results;
using System.Xml.Linq;

namespace onlineshopowner_api.Infrastructure.Repositories
{
    public class ShopRepository : IShopRepository
    {
        private readonly string _connectionstring;
        
       

        public ShopRepository()
        {
           
           
            _connectionstring = ConfigurationManager.ConnectionStrings["online_shopAdo"].ConnectionString;


        }
        public async Task<ResultCheckdb<int>> GetShopByShopOwnerid(int shopownerid)
        {
            Domain.Entities.shop shop;
            if (shopownerid == 0)
            {
                return new ResultCheckdb<int>
                {
                    IsSuccess = false,
                    Error = "the shopowner not in parametere",
                };
            }
            using (SqlConnection connection = new SqlConnection(_connectionstring))
            {
                connection.Open();
                string query = "SELECT top 1 shop_id FROM Shop WHERE shopowner_id = @shopownerId";
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@shopownerId", shopownerid);
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {

                            int shopid = reader.GetInt32(reader.GetOrdinal("shop_id"));
                               

                           
                            return new ResultCheckdb<int>
                            {
                                IsSuccess = true,
                                IsFound = true,
                                Value = shopid,
                                Error = "the shop is exist for this shopowner "

                            };
                        }
                        return new ResultCheckdb<int>
                        {
                            IsSuccess = true,
                            IsFound = false,
                            Error = "this shopowner is not have shop till now "
                        };
                    }




                }

            }
        }
        public async Task<ResultCheckdb<Domain.Entities.shop>> GetShopByShopOwner(Domain.Entities.ShopOwner shopowner)
        {
            Domain.Entities.shop shop;
            if (shopowner == null)
            {
                return new ResultCheckdb<Domain.Entities.shop>
                {
                    IsSuccess = false,
                    Error = "the shopowner not in parametere",
                };
            }
            using (SqlConnection connection = new SqlConnection(_connectionstring))
            {
                connection.Open();
                string query = "SELECT top 1 * FROM Shop WHERE shopowner_id = @shopownerId";
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@shopownerId", shopowner.ShopOwnerId);
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            shop = new Domain.Entities.shop
                            {
                                shopid = reader.GetInt32(reader.GetOrdinal("shop_id")),
                                name = reader.GetString(reader.GetOrdinal("name")),
                                logoUrl = reader["logo_url"] as string,
                                createddate = reader.GetDateTime(reader.GetOrdinal("created_date")),
                                description = reader["description"] as string,
                                shopownerid = reader.GetInt32(reader.GetOrdinal("shopowner_id"))

                            };
                            return new ResultCheckdb<Domain.Entities.shop>
                            {
                                IsSuccess = true,
                                IsFound = true,
                                Value = shop,
                                Error = "the shop is exist for this shopowner "

                            };
                        }
                        return new ResultCheckdb<Domain.Entities.shop>
                        {
                            IsSuccess = true,
                            IsFound = false,
                            Error = "this shopowner is not have shop till now "
                        };
                    }




                }

            }
        }


        public async Task<string> createShoponDatabase(Domain.Entities.shop shop)
        {
            if (shop == null)
            {
                return "yourdatanull";
            }
            try
            {
                using (SqlConnection connection = new SqlConnection(_connectionstring))
                {

                    string query = "INSERT INTO shop(name, description, shopowner_id) VALUES(@name, @description, @shopowner_id)";
                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@name", shop.name);
                        command.Parameters.AddWithValue("@description", shop.description);
                        command.Parameters.AddWithValue("@shopowner_id", shop.shopownerid);

                        connection.Open();
                        command.ExecuteNonQuery();
                        connection.Close();
                        return "Success";
                    }
                }
            }
            catch (Exception ex)
            {
                return ex.Message + ex.StackTrace;
            }
        }

        public async Task<UpdateDataProcess> Updatethelogourl(string urllogo,string deletehash ,int shopid)
        {
            if (string.IsNullOrEmpty(urllogo))
            {
                return UpdateDataProcess.yourdatanull;
            }
            try
            {
                using (SqlConnection connection = new SqlConnection(_connectionstring))
                {

                    string query = "update shop  set logo_url=@urllogo , deletehashimage=@deletehash  where shop_id=@shopid  ";
                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@shopid", shopid);
                        command.Parameters.AddWithValue("@urllogo", urllogo);
                        command.Parameters.AddWithValue("@deletehash", deletehash);

                        connection.Open();
                       await  command.ExecuteNonQueryAsync();
                        connection.Close();
                        return UpdateDataProcess.Success;

                    }
                }
            }
            catch (Exception ex) 
            {
                return UpdateDataProcess.catchError;
            }
        }
        public async Task<ResultCheckdb<List<string>>> GetShopType(int offset, int limit,string search)
        {
            var result = new ResultCheckdb<List<string>>();

            try
            {
                using (SqlConnection connection = new SqlConnection(_connectionstring))
                {
                    await connection.OpenAsync();

                    // Get total distinct count
                    string countQuery = "SELECT COUNT(DISTINCT type) FROM shop";
                    using (var countCommand = new SqlCommand(countQuery, connection))
                    {
                        result.totalCount = (int)await countCommand.ExecuteScalarAsync();
                    }


                    // Get paged data

                    string query = @"
                SELECT DISTINCT type
                FROM shop
                 where type like '%'+@search+'%'
                ORDER BY type DESC
                OFFSET @Offset ROWS FETCH NEXT @Limit ROWS ONLY";

                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        

                        command.Parameters.AddWithValue("@Offset", offset);
                        command.Parameters.AddWithValue("@Limit", limit);
                        command.Parameters.AddWithValue("@Search", search ?? "");

                        using (SqlDataReader reader = await command.ExecuteReaderAsync())
                        {
                            var types = new List<string>();

                            while (await reader.ReadAsync())
                            {
                                types.Add(reader.GetString(0));
                            }
                            result.pageSize = types.Count;
                            result.IsSuccess = true;
                            result.Value = types;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                result.IsSuccess = false;
                result.Error = ex.Message;
            }

            return result;
        }
        public async Task<ResultCheckdb<List<ShopSumaryDto>>> GetShoptouser(int limit = 20, int offset = 0, string searchbyshopname = null, string searchbyshoptype = null)
        {
            var result = new ResultCheckdb<List<ShopSumaryDto>>();
            try
            {
                using (SqlConnection connection = new SqlConnection(_connectionstring))
                {
                  await  connection.OpenAsync();
                    string query;
                    if ( string.IsNullOrEmpty(searchbyshoptype) && string.IsNullOrEmpty(searchbyshopname))
                    {
                        query = "select  shop_id,name,logo_url,description,type from shop order by shop_id desc OFFSET @Offset ROWS FETCH NEXT @Limit ROWS ONLY ";

                    }
                    else
                    {
                        query = "select  shop_id,name,logo_url,description,type from shop where( @searchbyshopname is null or  name like '%'+@searchbyshopname+'%') or (@searchbyshoptype is null or type like '%'+@searchbyshoptype+'%') order by shop_id desc OFFSET @Offset ROWS FETCH NEXT @Limit ROWS ONLY  ";
                    }



                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@Offset", offset);
                        command.Parameters.AddWithValue("@Limit", limit);
                        command.Parameters.AddWithValue("@searchbyshopname", searchbyshopname ?? "");
                        command.Parameters.AddWithValue("@searchbyshoptype", searchbyshoptype ?? "");
                      


                        using (SqlDataReader reader = await command.ExecuteReaderAsync())
                        {
                            var shopsumarys = new List<ShopSumaryDto>();
                            while (await reader.ReadAsync())
                            {
                                shopsumarys.Add(
                                    new ShopSumaryDto
                                    {
                                        Id = reader.GetInt32(reader.GetOrdinal("shop_id")),
                                        Name = reader.GetString(reader.GetOrdinal("Name")),
                                        Description = reader.GetString(reader.GetOrdinal("Description")),
                                        url = reader.GetString(reader.GetOrdinal("logo_url")),
                                        type=reader.GetString(reader.GetOrdinal("Type")),
                                    });
                            }
                            result.Value = shopsumarys;
                            result.IsSuccess = true;

                        }
                        connection.Close();
                    }
                }
            }catch(Exception ex)
            {
                result.IsSuccess = false;
                result.Error = ex.Message;
            }
            return result;
        }

        public async Task<ResultCheckdb<(decimal shoplatitude, decimal shoplan)>> GetShopLocationById(int shopid)
        {
            try
            {
                using (SqlConnection connect = new SqlConnection(_connectionstring))
                {
                    await connect.OpenAsync();
                    string query = "select latitude,longitude from shop where shop_id=@shopid ";
                    using (SqlCommand command = new SqlCommand(query, connect))
                    {
                        command.Parameters.AddWithValue("@shopid", shopid);
                        using (SqlDataReader reader = await command.ExecuteReaderAsync())
                        {
                            if (reader.Read())
                            {
                                decimal latitude = reader.GetDecimal(reader.GetOrdinal("latitude"));
                                decimal longitude = reader.GetDecimal(reader.GetOrdinal("longitude"));

                                connect.Close();

                                return new ResultCheckdb<(decimal shoplatitude, decimal shoplan)>
                                {
                                    IsFound = true,
                                    IsSuccess = true,
                                    Value = (latitude, longitude)
                                };
                            }
                            return new ResultCheckdb<(decimal shoplatitude, decimal shoplan)>
                            {
                                IsFound = false,
                                IsSuccess = true,

                            };


                        }
                    }
                }
            }
            catch (Exception ex) 
            {
                return new ResultCheckdb<(decimal shoplatitude, decimal shoplan)>
                {
                    IsSuccess = false
                };
            }
        }

        public async Task<ResultCheckdb<(string phonenumber,string email,string shopname)>> GetPhoneNumberAndEmailbyShopid(int shopid)
        {
            try
            {
                using (SqlConnection connect = new SqlConnection(_connectionstring))
                {
                    string query = "select p.phone_number,p.email,sh.name from person p,shopowner so ,shop sh where p.person_id=so.person_id and so.shopowner_id=sh.shopowner_id and sh.shop_id=@shopid ";
                    using (SqlCommand command = new SqlCommand(query, connect))
                    {

                        command.Parameters.AddWithValue("@shopid",shopid);
                        using (SqlDataReader reader = await command.ExecuteReaderAsync())

                        {
                            if (reader.Read()) 
                            {
                                string phonenumber = reader.GetString(reader.GetOrdinal("phone_number"));
                                string email=reader.GetString(reader.GetOrdinal("email"));
                                string name=reader.GetString(reader.GetOrdinal("name"));

                                return new ResultCheckdb<(string phonenumber, string email,string name)>
                                {
                                    IsFound = true,
                                    IsSuccess = true,
                                    Value = (phonenumber, email,name)
                                };
                            }
                            return new ResultCheckdb<(string phonenumber, string email,string name)>
                            {
                                IsFound = false,
                                IsSuccess = true
                            };
                        }


                    }
                }
               
            
            }
           
            
            catch (Exception ex) 
            {
                return new ResultCheckdb<(string phonenumber, string email,string name)>
                {
                    IsSuccess = false
                };
            }
        } 
        

    }
}

