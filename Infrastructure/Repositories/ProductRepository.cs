using onlineshopowner_api.Application.Dtos;
using onlineshopowner_api.Domain.Constant;
using onlineshopowner_api.Domain.Entities;
using onlineshopowner_api.Domain.Interfaces.IRepository;
using Pipelines.Sockets.Unofficial;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.ComTypes;
using System.Threading.Tasks;
using System.Web;

namespace onlineshopowner_api.Infrastructure.Repositories
{
    public class ProductRepository : IProductRepository
    {
        private readonly string _connectionstring;

        public ProductRepository()
        {
            _connectionstring = ConfigurationManager.ConnectionStrings["online_shopAdo"].ConnectionString;
        }
        /*  public ResultCheckdb<Domain.Entities.Product> GetProduct(string name = null, string categoryname = null, string description = null, int price = 0, int categoryid = 0, int shopid = 0, int productid = 0) 
          {
              if (!string.IsNullOrEmpty(name)&& !string.IsNullOrEmpty(categoryname)&&!string.IsNullOrEmpty(description))
              { 
              }
          }*/
        public async Task<ResultCheckdb<int>> GetProductid(string productname, int shopid)
        {
            if (string.IsNullOrEmpty(productname))
            {
                return new ResultCheckdb<int>
                {
                    IsSuccess = false,
                    Error = "product name is not exist "
                };
            }
            try
            {
                using (SqlConnection connect = new SqlConnection(_connectionstring))
                {
                    await connect.OpenAsync();
                    string query;

                    query = "select top 1 product_id from product where name=@productname and shop_id=@shopid ";

                    using (SqlCommand command = new SqlCommand(query, connect))
                    {
                        command.Parameters.AddWithValue("@productname", productname);
                        command.Parameters.AddWithValue("@shopid", shopid);

                        using (SqlDataReader reader = await command.ExecuteReaderAsync())
                        {
                            if (reader.Read())
                            {
                                int productid = reader.GetInt32(0);
                                connect.Close();
                                return new ResultCheckdb<int>
                                {
                                    IsSuccess = true,
                                    IsFound = true,
                                    Value = productid
                                };
                            }
                            return new ResultCheckdb<int>
                            {
                                IsSuccess = true,
                                IsFound = false

                            };
                        }
                    }

                }
            }
            catch (Exception ex)
            {
                return new ResultCheckdb<int>
                {
                    IsSuccess = false
                };
            }

        }

        public async Task<(bool issuccess, string messege)> addproduct(Domain.Entities.Product product)
        {
            try
            {
                if (product == null) return (false, null);
                using (SqlConnection connect = new SqlConnection(_connectionstring))
                {
                    await connect.OpenAsync();
                    string query;
                    query = "insert into product(name,price,description,category_id,shop_id) values (@name,@price,@description,@category_id,@shop_id)";
                    using (SqlCommand command = new SqlCommand(query, connect))
                    {
                        command.Parameters.AddWithValue("@name", product.name);
                        command.Parameters.AddWithValue("@price", product.price);
                        command.Parameters.AddWithValue("@description", product.description);
                        command.Parameters.AddWithValue("@category_id", product.category_id);
                        command.Parameters.AddWithValue("@shop_id", product.shop_id);

                        await command.ExecuteNonQueryAsync();
                        connect.Close();
                        return (true, "success");
                    }
                }
            }
            catch (Exception ex)
            {
                return (false, ex.Message);
            }
        }

        public async Task<(bool issuccess, string message)> AddUrlImageToProductImages(string imgurl, string deletehash, int productid,bool isprofile)
        {
            try
            {
                if (string.IsNullOrEmpty(imgurl)) { return (false, "url is empty"); }
                if (string.IsNullOrEmpty(deletehash)) { return (false, "delehashempty"); }
                using (SqlConnection connect = new SqlConnection(_connectionstring))
                {
                    await connect.OpenAsync();
                    string query;

                    using (SqlCommand command = new SqlCommand("addproductimage", connect))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@productid", productid);
                        command.Parameters.AddWithValue("@imgurl", imgurl);
                        command.Parameters.AddWithValue("@deletehash", deletehash);
                        command.Parameters.AddWithValue("@isprofile", isprofile);
                        command.Parameters.Add("@Success", SqlDbType.Bit).Direction = ParameterDirection.Output;
                        command.Parameters.Add("@ErrorMessage", SqlDbType.NVarChar, 4000).Direction = ParameterDirection.Output;
                        await command.ExecuteNonQueryAsync();
                        bool success = (bool)command.Parameters["@Success"].Value;
                        string errorMsg = command.Parameters["@ErrorMessage"].Value.ToString();
                        connect.Close();
                        if (success)
                        {
                            return (true, "yes");
                        }
                        else
                        {
                            return (false, errorMsg);

                        }
                    }
                }
            }
            catch (Exception ex)
            {
                return (false, ex.Message);
            }
        }

        public async Task<ResultCheckdb<Domain.Entities.Product>> GetProductById(int productid)
        {

            try
            {
                using (SqlConnection connect = new SqlConnection(_connectionstring))
                {
                   await connect.OpenAsync();

                    string query = "select * from product where product_id=@productid";
                    using (SqlCommand command = new SqlCommand(query, connect))
                    {
                        command.Parameters.AddWithValue("@productid", productid);
                        using (SqlDataReader reader = await command.ExecuteReaderAsync())
                        {
                            while (reader.Read())
                            {
                                var product = new Domain.Entities.Product
                                {
                                    product_id = reader.GetInt32(reader.GetOrdinal("product_id")),
                                    name = reader.GetString(reader.GetOrdinal("name")),
                                    price = reader.GetDecimal(reader.GetOrdinal("price")),
                                    description = reader.GetString(reader.GetOrdinal("description")),
                                    category_id = reader.GetInt32(reader.GetOrdinal("category_id")),
                                    shop_id = reader.GetInt32(reader.GetOrdinal("shop_id")),
                                    quentity=reader.GetInt32(reader.GetOrdinal("quantity"))


                                };
                                int statusindex = reader.GetOrdinal("status");
                                if (!reader.IsDBNull(statusindex))
                                {
                                    product.status=reader.GetString(statusindex);
                                }
                                connect.Close();
                                return new ResultCheckdb<Domain.Entities.Product>
                                {
                                    Value = product,
                                    IsFound = true,
                                    IsSuccess = true

                                };
                            };
                            return new ResultCheckdb<Domain.Entities.Product>
                            {
                                IsSuccess = true,
                                IsFound = false
                            };

                        }
                    }
                }
            }
            catch (Exception ex)
            {
                return new ResultCheckdb<Domain.Entities.Product>
                {
                    IsSuccess = false,
                    Error=ex.Message
                    
                };
            }
        }
        public async Task<ResultCheckdb<List<Domain.Entities.Product>>> GetproducToUser(int shopid = 0, int limit = 30,int offset=0, string searchbyproductname = null, string searchbycategory = null, string searchbyshoptype = null)
        {
            var result = new ResultCheckdb<List<Product>> 
            {
                Value=new List<Product>()
            };
            try
            {
                using (SqlConnection connect = new SqlConnection(_connectionstring))
                {
                    await connect.OpenAsync();

                    using (SqlCommand cmd = new SqlCommand("getproduct", connect))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@shopid", shopid);
                        cmd.Parameters.AddWithValue("@limit", limit);
                        cmd.Parameters.AddWithValue("@offset", offset);
                        cmd.Parameters.AddWithValue("@searchbyproductname", searchbyproductname);
                        cmd.Parameters.AddWithValue("@searchbycategory", searchbycategory);
                      
                        using (SqlDataReader reader =await cmd.ExecuteReaderAsync())
                        {

                            while (reader.Read())
                            {

                                    Product product = new Product();
                                    product.product_id = reader.GetInt32(reader.GetOrdinal("product_id"));
                                    product.name = reader.GetString(reader.GetOrdinal("name"));
                                    product.price = reader.GetDecimal(reader.GetOrdinal("price"));
                                    int descIndex = reader.GetOrdinal("description");
                                    if (!reader.IsDBNull(descIndex))
                                        product.description = reader.GetString(descIndex);
                                int imgindex = reader.GetOrdinal("imgurl_id");
                                if(!reader.IsDBNull(imgindex))
                                    product.imgurid=reader.GetInt32(imgindex);
                                    product.category_id = reader.GetInt32(reader.GetOrdinal("category_id"));
                                    product.shop_id = reader.GetInt32(reader.GetOrdinal("shop_id"));
                                if (!reader.IsDBNull(reader.GetOrdinal("status")))
                                    product.status = reader.GetString(reader.GetOrdinal("status"));
                                result.Value.Add(product);
                                

                            
                            }
                            connect.Close();
                            result.IsSuccess = true;

                        }
                    }
                }


            }
            catch (Exception ex)
            {
                return new ResultCheckdb<List<Product>>
                {
                    IsSuccess = false,
                    Error = ex.Message

                };
            }
           
            return result;
        }
    }
}            