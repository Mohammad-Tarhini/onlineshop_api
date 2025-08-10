using Antlr.Runtime.Tree;
using Microsoft.Ajax.Utilities;
using Microsoft.AspNet.SignalR.Messaging;
using onlineshopowner_api.Application.Dtos;
using onlineshopowner_api.Application.Dtos.PaymentAndOrder;
using onlineshopowner_api.Domain.Constant;
using onlineshopowner_api.Domain.Entities.PaymentAndOrder;
using onlineshopowner_api.Domain.Interfaces.IRepository;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Drawing.Text;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace onlineshopowner_api.Infrastructure.Repositories
{
    public class PaymentAndOrderRepository : IPaymentAndOrderRepository
    {
        private string connectionstring { get; set; }
        public PaymentAndOrderRepository()
        {
            connectionstring = ConfigurationManager.ConnectionStrings["online_shopAdo"].ConnectionString;
        }

        public async Task<(bool issucess, string message)> RegisterOrder(int personid, decimal total_price, decimal DeliveryCost, decimal latitude, decimal longitude, int shopid, int deliveryid, DataTable producttable,string shopdeliverypin,string clientdeliverypin,string paymentmethode)
        {
            try
            {

                using (SqlConnection connect = new SqlConnection(connectionstring))
                {
                    await connect.OpenAsync();
                    using (SqlCommand cmd = new SqlCommand("insertorderafterpayment", connect))
                    {
                        cmd.CommandType = System.Data.CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@personid", personid);
                        cmd.Parameters.AddWithValue("@paymentmethod", paymentmethode);
                        cmd.Parameters.AddWithValue("@shopid", shopid);
                        cmd.Parameters.AddWithValue("@totalprice", total_price);
                        cmd.Parameters.AddWithValue("@deliverycost", DeliveryCost);
                        cmd.Parameters.AddWithValue("@latitude", latitude);
                        cmd.Parameters.AddWithValue("@longitude", longitude);
                        cmd.Parameters.AddWithValue("@deliveryid", deliveryid);
                        cmd.Parameters.AddWithValue("@shopdeliverypin", shopdeliverypin);
                        cmd.Parameters.AddWithValue("@clientdeliverypin", clientdeliverypin);
                       
                        SqlParameter productparam = cmd.Parameters.AddWithValue("@products", producttable);
                        productparam.SqlDbType = SqlDbType.Structured;
                        productparam.TypeName = "dbo.products";

                        await cmd.ExecuteNonQueryAsync();
                        return (true, "success");
                    }

                }
            }
            catch (Exception ex)
            {
                return (false, ex.Message);
            }
        }
        public async Task<ResultCheckdb<List<Domain.Entities.PaymentAndOrder.Order>>> GetNewOrder(int  Shopid)
        {
            try
            {
                if (Shopid == 0) 
                { 
                    return new ResultCheckdb<List<Domain.Entities.PaymentAndOrder.Order> >
                      {
                        IsSuccess=false,
                        Error="no "

                        
                      };
                }

                using (SqlConnection connect = new SqlConnection(connectionstring))
                {
                    await connect.OpenAsync();
                    using (SqlCommand cmd = new SqlCommand("GetNeworder", connect))
                    {
                        cmd.CommandType = System.Data.CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@shopid", Shopid);

                        using(var reader=await cmd.ExecuteReaderAsync())
                        
                        {
                            var orders=new List<Domain.Entities.PaymentAndOrder.Order>();
                            while (reader.Read()) 
                            {
                                int orderid = reader.GetInt32(reader.GetOrdinal("order_id"));
                                var order_date = reader.GetDateTime(reader.GetOrdinal("order_date"));
                                string provider_type = reader.GetString(reader.GetOrdinal("provider_type"));
                                string deliveryname = reader.GetString(reader.GetOrdinal("deliveryname"));

                                string deliveryphonenumber = reader.GetString(reader.GetOrdinal("Phonenumber"));
                                string deliveryemail = reader.GetString(reader.GetOrdinal("email"));
                                int deliveryid = reader.GetInt32(reader.GetOrdinal("delivery_id"));

                                var order = new Domain.Entities.PaymentAndOrder.Order
                                {
                                    OrderId = orderid,
                                    deliveryemail = deliveryemail,
                                    deliveryphone = deliveryphonenumber,
                                    deliveryname = deliveryname,
                                    deliveryid = deliveryid
                                };
                                orders.Add(order);


                        
                            }
                            connect.Close();

                            if (orders==null || orders.Count == 0)
                            {
                                return new ResultCheckdb<List<Domain.Entities.PaymentAndOrder.Order>>
                                {
                                    IsSuccess = true,
                                    IsFound = false
                                };

                            }
                            return new ResultCheckdb<List<Domain.Entities.PaymentAndOrder.Order>>
                            {
                                IsFound = true,
                                IsSuccess = true,
                                Value = orders
                            };


                        }
                    }
                }
            } catch (Exception ex) 
            {
                return new ResultCheckdb<List<Domain.Entities.PaymentAndOrder.Order>>
                {
                    IsSuccess = false,
                    IsFound = false,
                    Error=ex.Message
                    
                };
            }
        }

        public async Task<ResultCheckdb<List<CartItem>>> GetItemsOfOrder(int orderid)
        {
            try
            {
                using(SqlConnection connect=new SqlConnection(connectionstring))
                {
                    await connect.OpenAsync();
                    using(SqlCommand command =new SqlCommand("GetItemOfOrder", connect))
                    {
                        var orderItems=new List<CartItem>();
                        command.CommandType = System.Data.CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@orderid",orderid);
                        using (SqlDataReader reader = command.ExecuteReader()) 
                        {
                            while (reader.Read()) 
                            {
                                int productid = reader.GetInt32(reader.GetOrdinal("product_id"));
                                int quantity = reader.GetInt32(reader.GetOrdinal("quantity"));
                                string productname = reader.GetString(reader.GetOrdinal("name"));
                                string description=reader.GetString(reader.GetOrdinal("description"));
                                var cartItem = new CartItem
                                {
                                    description = description,
                                    ProductId = productid,
                                    ProductName = productname,
                                    Quantity = quantity,
                                };
                                orderItems.Add(cartItem);
                            }
                            connect.Close();
                            if (orderItems.Count > 0) 
                            {
                                return new ResultCheckdb<List<CartItem>>
                                {
                                    IsFound = true,
                                    IsSuccess=true,
                                    Value= orderItems
                                    
                                };
                            }
                            else
                            {
                                return new ResultCheckdb<List<CartItem>>
                                {
                                    IsSuccess = true,
                                    IsFound = false,
                                    Value = orderItems
                                };
                            }
                        }
                    }
                }

            }
            catch (Exception ex) 
            {
                return new ResultCheckdb<List<CartItem>>
                {
                    IsFound = false,
                    Error = ex.ToString()
                };
            }
        }
        public async Task<ResultCheckdb<List<OrderForDelivery>>>GetOrdersForDelivery(int deliveryId)
        {
            try
            {
                if (deliveryId == 0) 
                {
                    return new ResultCheckdb<List<OrderForDelivery>>
                    {
                        IsFound = false,
                        IsSuccess = false,
                        Error = "no delivery id"

                    };
                }
                using(SqlConnection connect=new SqlConnection(connectionstring))
                {
                    await connect.OpenAsync();
                    using(SqlCommand cmd = new SqlCommand("GetOrdersForDelivery", connect))
                    {
                        cmd.CommandType= System.Data.CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@deliveryid",deliveryId);
                        using (var reader = await cmd.ExecuteReaderAsync()) 
                        {
                            var listorder = new List<OrderForDelivery>();
                            while (reader.Read()) 
                            {
                                int orderid = reader.GetInt32(reader.GetOrdinal("order_id"));
                                string hashshopdeliverypin = reader.GetString(reader.GetOrdinal("shopdeliveryPin"));
                                decimal shoplatitude = reader.GetDecimal(reader.GetOrdinal("shoplatitude"));
                                decimal shoplongitude = reader.GetDecimal(reader.GetOrdinal("shopLongitude"));
                                string shopphonenumber = reader.GetString(reader.GetOrdinal("shopphonenumber"));
                                string shopemail = reader.GetString(reader.GetOrdinal("shopemail"));
                                string clientname = reader.GetString(reader.GetOrdinal("clientname"));
                                string clientemail = reader.GetString(reader.GetOrdinal("clientemail"));
                                string clientphonenumber = reader.GetString(reader.GetOrdinal("clientphonenumber"));
                                var orderdate = reader.GetDateTime(reader.GetOrdinal("order_date"));
                                var shopname = reader.GetString(reader.GetOrdinal("shopname"));
                                var shopid = reader.GetInt32(reader.GetOrdinal("shop_id"));
                                var clientlatitude = reader.GetDecimal(reader.GetOrdinal("clientlatitude"));
                                var clientlongitude = reader.GetDecimal(reader.GetOrdinal("clintlongitude"));
                                var ordersfordelivery = new OrderForDelivery
                                {
                                    orderid = orderid,
                                    orderdate=orderdate,
                                    ShopEmail = shopemail,
                                    shopname=shopname,
                                    ShopPhoneNumber = shopphonenumber,
                                    clientname=clientname,
                                    clientemail=clientemail,
                                    clientphonenumber=clientphonenumber,
                                    shopid=shopid,
                                    clientlatitude=clientlatitude,
                                    clientlongitude=clientlongitude,
                                    shoplatitude=shoplatitude,
                                    shoplongitude=shoplongitude,
                                    HashDeliveryShopPin=hashshopdeliverypin
                                    

                                };
                                listorder.Add(ordersfordelivery);
                            }
                            if (listorder.Count == 0)
                            {
                                return new ResultCheckdb<List<OrderForDelivery>>
                                {
                                    IsSuccess = true,
                                    IsFound = false,
                                };
                            }
                            else
                            {
                                return new ResultCheckdb<List<OrderForDelivery>>
                                {
                                    IsFound = true,
                                    IsSuccess = true,
                                    Value = listorder
                                    
                                };
                            }
                        }
                    }

                }

            }
            catch (Exception ex) 
            {
                return new ResultCheckdb<List<OrderForDelivery>>
                {
                    IsFound = false,
                    IsSuccess = false,
                    Error = ex.Message+"r0"

                };
            }
        }

        public async Task<(bool issucess,bool isStartDelivery,string message)> deliveryreciveorder(RecievefromShopToDelivery recievefromShopToDelivery,int shopid) 
        {
            try
            {
                using (SqlConnection connect = new SqlConnection(connectionstring))
                {
                    await connect.OpenAsync();
                    //string query = "select shopid,delivery_id,shopdeliveryPin from client order where order_id=@orderid and shopdeliverypin=@shopdeliverypin";
                    using (SqlCommand cmd = new SqlCommand("recieveorderfromshoptoorder", connect))
                    {
                        cmd.CommandType= System.Data.CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@orderid", recievefromShopToDelivery.orderId);
                        cmd.Parameters.AddWithValue("@shopid", shopid);
                        cmd.Parameters.AddWithValue("@shopdeliverypin", recievefromShopToDelivery.password);
                        cmd.Parameters.AddWithValue("@delivery_id", recievefromShopToDelivery.deliveryid);

                        SqlParameter iscorrectParam = new SqlParameter("@iscorrectdelivery", SqlDbType.Bit)
                        {
                            Direction = ParameterDirection.Output
                        };
                        cmd.Parameters.Add(iscorrectParam);
                        await cmd.ExecuteReaderAsync();
                        var correctdelivery = Convert.ToBoolean(iscorrectParam.Value);
                        connect.Close();
                        if (correctdelivery == false) { return (true, false, "is not correct password"); }
                        else { return (true, true, "mabrouk"); }
                    }
                }
            }
            catch (Exception ex) 
            {
               return (false, false, ex.Message);
            }
        }

        public async Task<(bool issuccess,bool isfound,string message)> RecieveOrederFromDeliveryToClient(RecieveFromDeliveryToClient recievefromdeliverytoclient)
        {
            try
            {
                using (SqlConnection connect = new SqlConnection(connectionstring))
                {
                    await connect.OpenAsync();
                    using (SqlCommand command = new SqlCommand("Recieveorderfromdeliverytoclient",connect))
                    {
                        command.Parameters.AddWithValue("@orderid",recievefromdeliverytoclient.orderId);
                        command.Parameters.AddWithValue("@deliveryid", recievefromdeliverytoclient.deliveryid);
                        command.Parameters.AddWithValue("@clientdeliverypin",recievefromdeliverytoclient.password);

                        SqlParameter isexecuteparam = new SqlParameter("@isexecute",SqlDbType.Bit)
                        {
                            Direction = ParameterDirection.Output,
                        };
                        command.Parameters.Add(isexecuteparam);

                        await command.ExecuteNonQueryAsync();
                        var isexecuted=Convert.ToBoolean(isexecuteparam.Value);
                        connect.Close();
                        if (!isexecuted) 
                        {
                            return (true, false, "is not exist");
                        }
                        else
                        {
                            return (true, true, null);
                        }
                        
                    }
                }
            }
            catch (Exception ex) 
            {
                return (false, false, ex.Message);
            }
        }

    }
}