using onlineshopowner_api.Application.Dtos;
using onlineshopowner_api.Domain.Entities;
using onlineshopowner_api.Domain.Interfaces;
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

        public async Task<int?> GetShopIDByShopownerId(int shopOwnerId)
        {
            if (shopOwnerId == 0)
            {
                throw new ArgumentException("shop owner id is not valid");
            }
            using (SqlConnection connection = new SqlConnection(_connectionstring))
            {
                await connection.OpenAsync();
                string query = "SELECT top 1 shop_id FROM Shop WHERE shopowner_id = @shopownerId";
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@shopownerId", shopOwnerId);
                    using (SqlDataReader reader = await command.ExecuteReaderAsync())
                    {
                        if (reader.Read())
                        {
                            int shopid = reader.GetInt32(reader.GetOrdinal("shop_id"));
                            return shopid;
                        }
                        else
                        {
                            return null;
                        }
                    }
                }
                connection.Close();
            }

        }
        public async Task<Domain.Entities.shop> GetShopByShopOwnerIdOrShopId(int shopOwnerId = 0, int shopId = 0)
        {
            if (shopOwnerId == 0 && shopId == 0)
                throw new ArgumentException("Shop owner ID or Shop ID must be provided.");

            using (SqlConnection connection = new SqlConnection(_connectionstring))
            {
                await connection.OpenAsync();

                string query;

                if (shopId == 0)
                {
                    query = "SELECT TOP 1 * FROM Shop WHERE shopowner_id = @shopownerId";
                }
                else if (shopOwnerId == 0)
                {
                    query = "SELECT TOP 1 * FROM Shop WHERE shop_id = @shopId";
                }
                else
                {
                    throw new ArgumentException("Provide either shopOwnerId or shopId, not both.");
                }

                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    if (shopId == 0)
                        command.Parameters.AddWithValue("@shopownerId", shopOwnerId);
                    else
                        command.Parameters.AddWithValue("@shopId", shopId);

                    using (SqlDataReader reader = await command.ExecuteReaderAsync())
                    {
                        if (await reader.ReadAsync())
                        {
                            return new Domain.Entities.shop
                            {
                                shopid = reader.GetInt32(reader.GetOrdinal("shop_id")),
                                name = reader.GetString(reader.GetOrdinal("name")),
                                logoUrl = reader["logo_url"] as string,
                                createddate = reader.GetDateTime(reader.GetOrdinal("created_date")),
                                description = reader["description"] as string,
                                shopownerid = reader.GetInt32(reader.GetOrdinal("shopowner_id")),
                                deletehashingimage = reader["deletehashimage"] as string,
                                type = reader["type"] as string,
                                shoplatitude = reader.GetDecimal(reader.GetOrdinal("latitude")),
                                shopLongitude = reader.GetDecimal(reader.GetOrdinal("longitude"))
                            };
                        }

                        throw new InvalidOperationException("No shop found for the given ID.");
                    }
                }
            }
        }
        public async Task<int> AddShop(Domain.Entities.shop shop)
        {
            if (shop == null)
            {
                throw new ArgumentNullException(nameof(shop), "Shop cannot be null.");
            }

            using (SqlConnection connection = new SqlConnection(_connectionstring))
            {
                await connection.OpenAsync();
                string query = "INSERT INTO shop(name, description, shopowner_id,logo_url,deletehashimage,type,latitude,longitude) VALUES(@name, @description, @shopowner_id,@logo_url,@deletehashimage,@type,@latitude,@longitude); SELECT CAST(SCOPE_IDENTITY() AS INT)";
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@name", shop.name);
                    command.Parameters.AddWithValue("@description", shop.description);
                    command.Parameters.AddWithValue("@shopowner_id", shop.shopownerid);
                    command.Parameters.AddWithValue("@logo_url", shop.logoUrl);
                    command.Parameters.AddWithValue("@deletehashimage", shop.deletehashingimage);
                    command.Parameters.AddWithValue("@type", shop.type);
                    command.Parameters.AddWithValue("@latitude", shop.shoplatitude);
                    command.Parameters.AddWithValue("@longitude", shop.shopLongitude);

                    var result = await command.ExecuteScalarAsync();
                    if(result==null || result==DBNull.Value)
                        throw new InvalidOperationException("Failed to retrieve the new shop ID after insertion.");
                    connection.Close();
                    return Convert.ToInt32(result);
                    
                }
            }
        }

        public async Task updateShop(Domain.Entities.shop shop)
        {
            if (shop == null)
            {
                throw new ArgumentNullException(nameof(shop), "Shop cannot be null.");
            }
            using (SqlConnection connection = new SqlConnection(_connectionstring))
            {
                
                string query = "UPDATE shop SET name=@name, description=@description, logo_url=@logo_url, deletehashimage=@deletehashimage, type=@type, latitude=@latitude, longitude=@longitude WHERE shop_id=@shopid";
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@shopid", shop.shopid);
                    command.Parameters.AddWithValue("@name", shop.name);
                    command.Parameters.AddWithValue("@description", shop.description);
                    command.Parameters.AddWithValue("@logo_url", shop.logoUrl);
                    command.Parameters.AddWithValue("@deletehashimage", shop.deletehashingimage);
                    command.Parameters.AddWithValue("@type", shop.type);
                    command.Parameters.AddWithValue("@latitude", shop.shoplatitude);
                    command.Parameters.AddWithValue("@longitude", shop.shopLongitude);
                    await connection.OpenAsync();
                    await command.ExecuteNonQueryAsync();
                }
                connection.Close();

            }

        }
        public async Task<List<Domain.Entities.shop>> GetShops(
    int limit = 20,
    int offset = 0,
    string searchbyshopname = null,
    string searchbyshoptype = null)
        {
            var result = new List<shop>();

            using (SqlConnection connection = new SqlConnection(_connectionstring))
            {
                await connection.OpenAsync();

                string query;

                if (string.IsNullOrEmpty(searchbyshoptype) && string.IsNullOrEmpty(searchbyshopname))
                {
                    query = @"SELECT TOP (@limit) shop_id, name, logo_url, description, type, longitude, latitude
                      FROM shop
                      WHERE shop_id >= @offset
                      ORDER BY shop_id ASC";
                }
                else
                {
                    query = @"SELECT TOP (@limit) shop_id, name, logo_url, description, type, longitude, latitude
                      FROM shop
                      WHERE 
                      (
                          (@searchbyshopname = '' OR name LIKE '%' + @searchbyshopname + '%')
                          AND
                          (@searchbyshoptype = '' OR type LIKE '%' + @searchbyshoptype + '%')
                      )
                      AND shop_id >= @offset
                      ORDER BY shop_id ASC";
                }

                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@offset", offset);
                    command.Parameters.AddWithValue("@limit", limit);
                    command.Parameters.AddWithValue("@searchbyshopname", searchbyshopname ?? "");
                    command.Parameters.AddWithValue("@searchbyshoptype", searchbyshoptype ?? "");

                    using (SqlDataReader reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            result.Add(new shop
                            {
                                shopid = reader.GetInt32(reader.GetOrdinal("shop_id")),
                                name = reader.GetString(reader.GetOrdinal("name")),
                                description = reader["description"] as string,
                                logoUrl = reader["logo_url"] as string,
                                type = reader["type"] as string,
                                shopLongitude = reader.GetDecimal(reader.GetOrdinal("longitude")),
                                shoplatitude = reader.GetDecimal(reader.GetOrdinal("latitude"))
                            });
                        }
                    }
                }
            }

            return result;
        }




        public async Task<List<string>> GetShopType(int offset, int limit, string search)
        {
            var result = new List<string>();

            using (SqlConnection connection = new SqlConnection(_connectionstring))
            {
                await connection.OpenAsync();





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


                        while (await reader.ReadAsync())
                        {
                            result.Add(reader.GetString(0));
                        }
                        connection.Close();
                    }
                }
            }


            return result;
        }
        public async Task AddShopCategory(int shopid, List<int> categoryid)
        {
            if (shopid == 0 || categoryid == null || categoryid.Count == 0)
                throw new ArgumentException("shop id or category id is not valid");

            using (SqlConnection connection = new SqlConnection(_connectionstring))
            {
                await connection.OpenAsync();

                foreach (var catId in categoryid)
                {
                    string query = "INSERT INTO shopcategory (shop_id, category_id) VALUES (@shop_id, @category_id)";

                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@shop_id", shopid);
                        command.Parameters.AddWithValue("@category_id", catId);

                        await command.ExecuteNonQueryAsync();
                    }
                }
                    connection.Close();
            }
        }



        public async Task<(decimal shopLatitude, decimal shopLongitude)?> GetShopLocationById(int shopid)
        {
            using (SqlConnection connect = new SqlConnection(_connectionstring))
            {
                await connect.OpenAsync();

                string query = "select latitude, longitude from shop where shop_id = @shopid";

                using (SqlCommand command = new SqlCommand(query, connect))
                {
                    command.Parameters.AddWithValue("@shopid", shopid);

                    using (SqlDataReader reader = await command.ExecuteReaderAsync())
                    {
                        if (await reader.ReadAsync())
                        {
                            decimal latitude = reader.GetDecimal(reader.GetOrdinal("latitude"));
                            decimal longitude = reader.GetDecimal(reader.GetOrdinal("longitude"));

                            return (latitude, longitude);
                        }
                    }
                }
                connect.Close();
            }
          
            return null; // shop not found
        }
    }
}


//    public async Task<ResultCheckdb<int>> GetShopByShopOwnerid(int shopownerid)
//    {
//        Domain.Entities.shop shop;
//        if (shopownerid == 0)
//        {
//            return new ResultCheckdb<int>
//            {
//                IsSuccess = false,
//                Error = "the shopowner not in parametere",
//            };
//        }
//        using (SqlConnection connection = new SqlConnection(_connectionstring))
//        {
//            connection.Open();
//            string query = "SELECT top 1 shop_id FROM Shop WHERE shopowner_id = @shopownerId";
//            using (SqlCommand command = new SqlCommand(query, connection))
//            {
//                command.Parameters.AddWithValue("@shopownerId", shopownerid);
//                using (SqlDataReader reader = command.ExecuteReader())
//                {
//                    if (reader.Read())
//                    {

//                        int shopid = reader.GetInt32(reader.GetOrdinal("shop_id"));



//                        return new ResultCheckdb<int>
//                        {
//                            IsSuccess = true,
//                            IsFound = true,
//                            Value = shopid,
//                            Error = "the shop is exist for this shopowner "

//                        };
//                    }
//                    return new ResultCheckdb<int>
//                    {
//                        IsSuccess = true,
//                        IsFound = false,
//                        Error = "this shopowner is not have shop till now "
//                    };
//                }




//            }

//        }
//    }
//    public async Task<ResultCheckdb<Domain.Entities.shop>> GetShopByShopOwner(Domain.Entities.ShopOwner shopowner)
//    {
//        Domain.Entities.shop shop;
//        if (shopowner == null)
//        {
//            return new ResultCheckdb<Domain.Entities.shop>
//            {
//                IsSuccess = false,
//                Error = "the shopowner not in parametere",
//            };
//        }
//        using (SqlConnection connection = new SqlConnection(_connectionstring))
//        {
//            connection.Open();
//            string query = "SELECT top 1 * FROM Shop WHERE shopowner_id = @shopownerId";
//            using (SqlCommand command = new SqlCommand(query, connection))
//            {
//                command.Parameters.AddWithValue("@shopownerId", shopowner.ShopOwnerId);
//                using (SqlDataReader reader = command.ExecuteReader())
//                {
//                    if (reader.Read())
//                    {
//                        shop = new Domain.Entities.shop
//                        {
//                            shopid = reader.GetInt32(reader.GetOrdinal("shop_id")),
//                            name = reader.GetString(reader.GetOrdinal("name")),
//                            logoUrl = reader["logo_url"] as string,
//                            createddate = reader.GetDateTime(reader.GetOrdinal("created_date")),
//                            description = reader["description"] as string,
//                            shopownerid = reader.GetInt32(reader.GetOrdinal("shopowner_id"))

//                        };
//                        return new ResultCheckdb<Domain.Entities.shop>
//                        {
//                            IsSuccess = true,
//                            IsFound = true,
//                            Value = shop,
//                            Error = "the shop is exist for this shopowner "

//                        };
//                    }
//                    return new ResultCheckdb<Domain.Entities.shop>
//                    {
//                        IsSuccess = true,
//                        IsFound = false,
//                        Error = "this shopowner is not have shop till now "
//                    };
//                }




//            }

//        }
//    }


//    public async Task<string> createShoponDatabase(Domain.Entities.shop shop)
//    {
//        if (shop == null)
//        {
//            return "yourdatanull";
//        }
//        try
//        {
//            using (SqlConnection connection = new SqlConnection(_connectionstring))
//            {

//                string query = "INSERT INTO shop(name, description, shopowner_id) VALUES(@name, @description, @shopowner_id)";
//                using (SqlCommand command = new SqlCommand(query, connection))
//                {
//                    command.Parameters.AddWithValue("@name", shop.name);
//                    command.Parameters.AddWithValue("@description", shop.description);
//                    command.Parameters.AddWithValue("@shopowner_id", shop.shopownerid);

//                    connection.Open();
//                    command.ExecuteNonQuery();
//                    connection.Close();
//                    return "Success";
//                }
//            }
//        }
//        catch (Exception ex)
//        {
//            return ex.Message + ex.StackTrace;
//        }
//    }

//    public async Task<UpdateDataProcess> Updatethelogourl(string urllogo,string deletehash ,int shopid)
//    {
//        if (string.IsNullOrEmpty(urllogo))
//        {
//            return UpdateDataProcess.yourdatanull;
//        }
//        try
//        {
//            using (SqlConnection connection = new SqlConnection(_connectionstring))
//            {

//                string query = "update shop  set logo_url=@urllogo , deletehashimage=@deletehash  where shop_id=@shopid  ";
//                using (SqlCommand command = new SqlCommand(query, connection))
//                {
//                    command.Parameters.AddWithValue("@shopid", shopid);
//                    command.Parameters.AddWithValue("@urllogo", urllogo);
//                    command.Parameters.AddWithValue("@deletehash", deletehash);

//                    connection.Open();
//                   await  command.ExecuteNonQueryAsync();
//                    connection.Close();
//                    return UpdateDataProcess.Success;

//                }
//            }
//        }
//        catch (Exception ex) 
//        {
//            return UpdateDataProcess.catchError;
//        }
//    }
//    public async Task<ResultCheckdb<List<string>>> GetShopType(int offset, int limit,string search)
//    {
//        var result = new ResultCheckdb<List<string>>();

//        try
//        {
//            using (SqlConnection connection = new SqlConnection(_connectionstring))
//            {
//                await connection.OpenAsync();

//                // Get total distinct count
//                string countQuery = "SELECT COUNT(DISTINCT type) FROM shop";
//                using (var countCommand = new SqlCommand(countQuery, connection))
//                {
//                    result.totalCount = (int)await countCommand.ExecuteScalarAsync();
//                }


//                // Get paged data

//                string query = @"
//            SELECT DISTINCT type
//            FROM shop
//             where type like '%'+@search+'%'
//            ORDER BY type DESC
//            OFFSET @Offset ROWS FETCH NEXT @Limit ROWS ONLY";

//                using (SqlCommand command = new SqlCommand(query, connection))
//                {


//                    command.Parameters.AddWithValue("@Offset", offset);
//                    command.Parameters.AddWithValue("@Limit", limit);
//                    command.Parameters.AddWithValue("@Search", search ?? "");

//                    using (SqlDataReader reader = await command.ExecuteReaderAsync())
//                    {
//                        var types = new List<string>();

//                        while (await reader.ReadAsync())
//                        {
//                            types.Add(reader.GetString(0));
//                        }
//                        result.pageSize = types.Count;
//                        result.IsSuccess = true;
//                        result.Value = types;
//                    }
//                }
//            }
//        }
//        catch (Exception ex)
//        {
//            result.IsSuccess = false;
//            result.Error = ex.Message;
//        }

//        return result;
//    }
//    public async Task<ResultCheckdb<List<ShopSumaryDto>>> GetShoptouser(int limit = 20, int offset = 0, string searchbyshopname = null, string searchbyshoptype = null)
//    {
//        var result = new ResultCheckdb<List<ShopSumaryDto>>();
//        try
//        {
//            using (SqlConnection connection = new SqlConnection(_connectionstring))
//            {
//              await  connection.OpenAsync();
//                string query;
//                if ( string.IsNullOrEmpty(searchbyshoptype) && string.IsNullOrEmpty(searchbyshopname))
//                {
//                    query = "select  shop_id,name,logo_url,description,type from shop order by shop_id desc OFFSET @Offset ROWS FETCH NEXT @Limit ROWS ONLY ";

//                }
//                else
//                {
//                    query = "select  shop_id,name,logo_url,description,type from shop where( @searchbyshopname is null or  name like '%'+@searchbyshopname+'%') or (@searchbyshoptype is null or type like '%'+@searchbyshoptype+'%') order by shop_id desc OFFSET @Offset ROWS FETCH NEXT @Limit ROWS ONLY  ";
//                }



//                using (SqlCommand command = new SqlCommand(query, connection))
//                {
//                    command.Parameters.AddWithValue("@Offset", offset);
//                    command.Parameters.AddWithValue("@Limit", limit);
//                    command.Parameters.AddWithValue("@searchbyshopname", searchbyshopname ?? "");
//                    command.Parameters.AddWithValue("@searchbyshoptype", searchbyshoptype ?? "");



//                    using (SqlDataReader reader = await command.ExecuteReaderAsync())
//                    {
//                        var shopsumarys = new List<ShopSumaryDto>();
//                        while (await reader.ReadAsync())
//                        {
//                            shopsumarys.Add(
//                                new ShopSumaryDto
//                                {
//                                    Id = reader.GetInt32(reader.GetOrdinal("shop_id")),
//                                    Name = reader.GetString(reader.GetOrdinal("Name")),
//                                    Description = reader.GetString(reader.GetOrdinal("Description")),
//                                    url = reader.GetString(reader.GetOrdinal("logo_url")),
//                                    type=reader.GetString(reader.GetOrdinal("Type")),
//                                });
//                        }
//                        result.Value = shopsumarys;
//                        result.IsSuccess = true;

//                    }
//                    connection.Close();
//                }
//            }
//        }catch(Exception ex)
//        {
//            result.IsSuccess = false;
//            result.Error = ex.Message+"rrrr";
//        }
//        return result;
//    }

//    public async Task<ResultCheckdb<(decimal shoplatitude, decimal shoplan)>> GetShopLocationById(int shopid)
//    {
//        try
//        {
//            using (SqlConnection connect = new SqlConnection(_connectionstring))
//            {
//                await connect.OpenAsync();
//                string query = "select latitude,longitude from shop where shop_id=@shopid ";
//                using (SqlCommand command = new SqlCommand(query, connect))
//                {
//                    command.Parameters.AddWithValue("@shopid", shopid);
//                    using (SqlDataReader reader = await command.ExecuteReaderAsync())
//                    {
//                        if (reader.Read())
//                        {
//                            decimal latitude = reader.GetDecimal(reader.GetOrdinal("latitude"));
//                            decimal longitude = reader.GetDecimal(reader.GetOrdinal("longitude"));

//                            connect.Close();

//                            return new ResultCheckdb<(decimal shoplatitude, decimal shoplan)>
//                            {
//                                IsFound = true,
//                                IsSuccess = true,
//                                Value = (latitude, longitude)
//                            };
//                        }
//                        return new ResultCheckdb<(decimal shoplatitude, decimal shoplan)>
//                        {
//                            IsFound = false,
//                            IsSuccess = true,

//                        };


//                    }
//                }
//            }
//        }
//        catch (Exception ex) 
//        {
//            return new ResultCheckdb<(decimal shoplatitude, decimal shoplan)>
//            {
//                IsSuccess = false
//            };
//        }
//    }

//    public async Task<ResultCheckdb<(string phonenumber,string email,string shopname)>> GetPhoneNumberAndEmailbyShopid(int shopid)
//    {
//        try
//        {
//            using (SqlConnection connect = new SqlConnection(_connectionstring))
//            {
//                string query = "select p.phone_number,p.email,sh.name from person p,shopowner so ,shop sh where p.person_id=so.person_id and so.shopowner_id=sh.shopowner_id and sh.shop_id=@shopid ";
//                using (SqlCommand command = new SqlCommand(query, connect))
//                {

//                    command.Parameters.AddWithValue("@shopid",shopid);
//                    using (SqlDataReader reader = await command.ExecuteReaderAsync())

//                    {
//                        if (reader.Read()) 
//                        {
//                            string phonenumber = reader.GetString(reader.GetOrdinal("phone_number"));
//                            string email=reader.GetString(reader.GetOrdinal("email"));
//                            string name=reader.GetString(reader.GetOrdinal("name"));

//                            return new ResultCheckdb<(string phonenumber, string email,string name)>
//                            {
//                                IsFound = true,
//                                IsSuccess = true,
//                                Value = (phonenumber, email,name)
//                            };
//                        }
//                        return new ResultCheckdb<(string phonenumber, string email,string name)>
//                        {
//                            IsFound = false,
//                            IsSuccess = true
//                        };
//                    }


//                }
//            }


//        }


//        catch (Exception ex) 
//        {
//            return new ResultCheckdb<(string phonenumber, string email,string name)>
//            {
//                IsSuccess = false
//            };
//        }
//    } 


//}


