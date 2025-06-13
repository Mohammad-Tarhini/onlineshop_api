using onlineshopowner_api.Domain.Constant;
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

        public async Task<UpdateDataProcess> createShoponDatabase(Domain.Entities.shop shop)
        {
            if (shop == null)
            {
                return UpdateDataProcess.yourdatanull;
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
                        return UpdateDataProcess.Success;
                    }
                }
            }
            catch (Exception ex)
            {
                return UpdateDataProcess.catchError;
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


    }
}

