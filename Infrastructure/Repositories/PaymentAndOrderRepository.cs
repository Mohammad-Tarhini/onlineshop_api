using Antlr.Runtime.Tree;
using Microsoft.Ajax.Utilities;
using Microsoft.AspNet.SignalR.Messaging;
using onlineshopowner_api.Application.Dtos;
using onlineshopowner_api.Application.Dtos.PaymentAndOrder;
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
        //+++++++++++++++++new+++++++++++++++++++++++++++++++++++++++++++++
        public async Task<int> RegisterOrder(clientOrder clientOrder, List<OrderDetail> orderDetails)
        {
            var orderDetailsTable = new DataTable();
            orderDetailsTable.Columns.Add("productId", typeof(int));
            orderDetailsTable.Columns.Add("quantity", typeof(int));
            foreach (var detail in orderDetails)
            {
                orderDetailsTable.Rows.Add(detail.productId, detail.quantity);
            }

            using (SqlConnection connect = new SqlConnection(connectionstring))
            {
                await connect.OpenAsync();
                using (SqlCommand cmd = new SqlCommand("insertorderafterpayment", connect))//the name of the stored procedure that will insert the order and order details before payment 
                {
                    cmd.CommandType = System.Data.CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@clientId", clientOrder.clientId);
                    cmd.Parameters.AddWithValue("@shopid", clientOrder.shopId);
                    cmd.Parameters.AddWithValue("@totalpriceProduct", clientOrder.ProductTotalCost);
                    cmd.Parameters.AddWithValue("@deliverycost", clientOrder.DeliveryCost);
                    cmd.Parameters.AddWithValue("@latitude", clientOrder.latitude);
                    cmd.Parameters.AddWithValue("@longitude", clientOrder.longitude);
                    cmd.Parameters.AddWithValue("@deliveryid", clientOrder.deliveryProviderId);
                  //  cmd.Parameters.AddWithValue("@paymentmethod", paymentMethod);
                    SqlParameter orderidParam = new SqlParameter("@orderid", SqlDbType.Int)
                    {
                        Direction = ParameterDirection.Output
                    };
                    cmd.Parameters.Add(orderidParam);
                    await cmd.ExecuteNonQueryAsync();
                    int orderId = Convert.ToInt32(orderidParam.Value);
                    connect.Close();
                    return orderId;
                    
                }
            }
        }
        public async Task<Domain.Entities.PaymentAndOrder.clientOrder> GetOrderByOrderId(int OrderId)
        {
            using (SqlConnection con = new SqlConnection(connectionstring))
            {
               await con.OpenAsync();
                string sql = "select * from clientorder where order_id=@orderId ";

                using (SqlCommand cmd = new SqlCommand(sql, con))
                {
                    cmd.Parameters.AddWithValue("@orderId", OrderId);
                    using (SqlDataReader reader = await cmd.ExecuteReaderAsync())
                    {
                        if (reader.Read())
                        {
                            var order = new Domain.Entities.PaymentAndOrder.clientOrder
                            {
                                orderId = reader.GetInt32(reader.GetOrdinal("order_id")),
                                deliveryProviderId = reader.GetInt32(reader.GetOrdinal("delivery_id")),
                                shopId = reader.GetInt32(reader.GetOrdinal("shopid")),
                                clientId = reader.GetInt32(reader.GetOrdinal("client_id")),
                                latitude = reader.GetDecimal(reader.GetOrdinal("latitude")),
                                longitude = reader.GetDecimal(reader.GetOrdinal("longitude"))
                            };
                            con.Close();
                            return order;
                        }
                        else
                        {
                            con.Close();
                            return null;
                        }

                    }
                   
                }
            }
        }
        public async Task<List<Domain.Entities.PaymentAndOrder.clientOrder>> GetOrdersRequiredForsopOrDelivery(int ShopId = 0, int deliveryId = 0)
        {

            if (ShopId == 0 && deliveryId == 0)
            {
                throw new Exception("no data id");
            }

            using (SqlConnection connect = new SqlConnection(connectionstring))
            {
                await connect.OpenAsync();
                string query = "";
                if (ShopId != 0)
                {
                    query = "select * from  clientorder where shopid=@shopid and status='paid'";
                }
                else if (deliveryId != 0)
                {
                    query = "select * from from clientorder where delivery_id=@deliveryid and status='paid'";
                }
                using (SqlCommand cmd = new SqlCommand(query, connect))
                {
                    if (ShopId != 0 && deliveryId==0)
                    {
                        cmd.Parameters.AddWithValue("@shopid", ShopId);
                    }
                    else if(deliveryId != 0 && ShopId==0)
                    {
                        cmd.Parameters.AddWithValue("@deliveryid", deliveryId);
                    }

                        

                    using (var reader = await cmd.ExecuteReaderAsync())

                    {
                        var orders = new List<Domain.Entities.PaymentAndOrder.clientOrder>();
                        while (reader.Read())
                        {
                            var clientorder = new Domain.Entities.PaymentAndOrder.clientOrder
                            {
                                orderId = reader.GetInt32(reader.GetOrdinal("order_id")),
                                deliveryProviderId = reader.GetInt32(reader.GetOrdinal("delivery_id")),
                                shopId = reader.GetInt32(reader.GetOrdinal("shopid")),
                                clientId = reader.GetInt32(reader.GetOrdinal("client_id")),
                                latitude = reader.GetDecimal(reader.GetOrdinal("latitude")),
                                longitude = reader.GetDecimal(reader.GetOrdinal("longitude")),
                                ProductTotalCost = reader.GetDecimal(reader.GetOrdinal("total_price")),
                                DeliveryCost = reader.GetDecimal(reader.GetOrdinal("deliverycost")),
                                orderDate = reader.GetDateTime(reader.GetOrdinal("order_date")),
                                orderStatus = reader.GetString(reader.GetOrdinal("status")),
                                shopdeliverypin = reader.GetString(reader.GetOrdinal("shopdeliverypin")),
                                totalPrice = reader.GetDecimal(reader.GetOrdinal("total_price"))

                            };
                            orders.Add(clientorder);




                        }
                        connect.Close();
                        return orders;


                    }

                }
            }
        }
        public async Task updateStatusOnClientOrder(int orderId, string status)
        {
            using (SqlConnection connection = new SqlConnection(connectionstring))
            {
                await connection.OpenAsync();
                string query = "update clientorder set status=@status  where order_id=@orderId ";
                using (SqlCommand cmd = new SqlCommand(query, connection))
                {
                    cmd.Parameters.AddWithValue("@OrderId", orderId);
                    cmd.Parameters.AddWithValue("@status", status);
                    await cmd.ExecuteNonQueryAsync();
                }
                connection.Close();
            }
        }
        public async Task<List<OrderDetail>> GetItemsOfOrder(int orderid)
        {
            using (SqlConnection connect = new SqlConnection(connectionstring))
            {
                await connect.OpenAsync();
                string query = "select order_detail_id, product_id,quantity from orderdetail where order_id=@orderid";
                using (SqlCommand command = new SqlCommand(query, connect))
                {
                    var orderItems = new List<OrderDetail>();

                    command.Parameters.AddWithValue("@orderid", orderid);
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            int productid = reader.GetInt32(reader.GetOrdinal("product_id"));
                            int quantity = reader.GetInt32(reader.GetOrdinal("quantity"));
                            int orderdetailid = reader.GetInt32(reader.GetOrdinal("order_detail_id"));
                            var orderItem = new OrderDetail
                            {
                                Id = orderdetailid,

                                productId = productid,

                                quantity = quantity,
                            };
                            orderItems.Add(orderItem);
                        }
                        connect.Close();
                        return orderItems;

                    }
                }
            }

        }
        public async Task deliveryreciveorder(RecievefromShopToDeliveryDto recievefromShopToDelivery, int shopid)
        {

            using (SqlConnection connect = new SqlConnection(connectionstring))
            {
                await connect.OpenAsync();
                //string query = "select shopid,delivery_id,shopdeliveryPin from client order where order_id=@orderid and shopdeliverypin=@shopdeliverypin";
                using (SqlCommand cmd = new SqlCommand("recieveorderfromshoptoorder", connect))
                {
                    cmd.CommandType = System.Data.CommandType.StoredProcedure;
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
                    if (!correctdelivery)
                    {
                        throw new Exception("order id or shop delivery pin is incorrect");
                    }
                    connect.Close();

                }
            }


        }
        public async Task RecieveOrederFromDeliveryToClient(RecieveFromDeliveryToClientDto recievefromdeliverytoclient)
        {

            using (SqlConnection connect = new SqlConnection(connectionstring))
            {
                await connect.OpenAsync();
                using (SqlCommand command = new SqlCommand("Recieveorderfromdeliverytoclient", connect))
                {
                    command.Parameters.AddWithValue("@orderid", recievefromdeliverytoclient.orderId);
                    command.Parameters.AddWithValue("@deliveryid", recievefromdeliverytoclient.deliveryid);
                    command.Parameters.AddWithValue("@clientdeliverypin", recievefromdeliverytoclient.password);

                    SqlParameter isexecuteparam = new SqlParameter("@isexecute", SqlDbType.Bit)
                    {
                        Direction = ParameterDirection.Output,
                    };
                    command.Parameters.Add(isexecuteparam);

                    await command.ExecuteNonQueryAsync();
                    var isexecuted = Convert.ToBoolean(isexecuteparam.Value);
                    connect.Close();
                    if (!isexecuted)
                    {
                        throw new Exception("order id or delivery id or client delivery pin is incorrect");
                    }


                }
            }
        }
    }
}






        //+++++++++++++++++Last+++++++++++++++++++++++++++++++++++++++++++++++++

//        public async Task<(bool issucess, string message)> RegisterOrder(int personid, decimal total_price, decimal DeliveryCost, decimal latitude, decimal longitude, int shopid, int deliveryid, DataTable producttable,string shopdeliverypin,string clientdeliverypin,string paymentmethode)
//        {
//            try
//            {

//                using (SqlConnection connect = new SqlConnection(connectionstring))
//                {
//                    await connect.OpenAsync();
//                    using (SqlCommand cmd = new SqlCommand("insertorderafterpayment", connect))
//                    {
//                        cmd.CommandType = System.Data.CommandType.StoredProcedure;
//                        cmd.Parameters.AddWithValue("@personid", personid);
//                        cmd.Parameters.AddWithValue("@paymentmethod", paymentmethode);
//                        cmd.Parameters.AddWithValue("@shopid", shopid);
//                        cmd.Parameters.AddWithValue("@totalprice", total_price);
//                        cmd.Parameters.AddWithValue("@deliverycost", DeliveryCost);
//                        cmd.Parameters.AddWithValue("@latitude", latitude);
//                        cmd.Parameters.AddWithValue("@longitude", longitude);
//                        cmd.Parameters.AddWithValue("@deliveryid", deliveryid);
//                        cmd.Parameters.AddWithValue("@shopdeliverypin", shopdeliverypin);
//                        cmd.Parameters.AddWithValue("@clientdeliverypin", clientdeliverypin);
                       
//                        SqlParameter productparam = cmd.Parameters.AddWithValue("@products", producttable);
//                        productparam.SqlDbType = SqlDbType.Structured;
//                        productparam.TypeName = "dbo.products";

//                        await cmd.ExecuteNonQueryAsync();
//                        return (true, "success");
//                    }

//                }
//            }
//            catch (Exception ex)
//            {
//                return (false, ex.Message);
//            }
//        }
      
          
//        public async Task<List<OrderDetail>> GetItemsOfOrder(int orderid)
//        {
//              using(SqlConnection connect=new SqlConnection(connectionstring))
//                {
//                    await connect.OpenAsync();
//                    string query = "select order_detail_id, product_id,quantity from orderdetail where order_id=@orderid";
//                    using (SqlCommand command =new SqlCommand(query, connect))
//                    {
//                        var orderItems=new List<OrderDetail>();
                   
//                        command.Parameters.AddWithValue("@orderid",orderid);
//                        using (SqlDataReader reader = command.ExecuteReader()) 
//                        {
//                            while (reader.Read()) 
//                            {
//                                int productid = reader.GetInt32(reader.GetOrdinal("product_id"));
//                                int quantity = reader.GetInt32(reader.GetOrdinal("quantity"));
//                                int orderdetailid = reader.GetInt32(reader.GetOrdinal("order_detail_id"));
//                                var orderItem = new OrderDetail
//                                {
//                                    Id=orderdetailid,

//                                    productId = productid,
                                  
//                                    quantity = quantity,
//                                };
//                                orderItems.Add(orderItem);
//                            }
//                            connect.Close();
//                            return orderItems;

//                        }
//                    }
//                }

//            }
           
//        }
//        public async Task<ResultCheckdb<List<OrderForDelivery>>>GetOrdersForDelivery(int deliveryId)
//        {
//            try
//            {
//                if (deliveryId == 0) 
//                {
//                    return new ResultCheckdb<List<OrderForDelivery>>
//                    {
//                        IsFound = false,
//                        IsSuccess = false,
//                        Error = "no delivery id"

//                    };
//                }
//                using(SqlConnection connect=new SqlConnection(connectionstring))
//                {
//                    await connect.OpenAsync();
//                    using(SqlCommand cmd = new SqlCommand("GetOrdersForDelivery", connect))
//                    {
//                        cmd.CommandType= System.Data.CommandType.StoredProcedure;
//                        cmd.Parameters.AddWithValue("@deliveryid",deliveryId);
//                        using (var reader = await cmd.ExecuteReaderAsync()) 
//                        {
//                            var listorder = new List<OrderForDelivery>();
//                            while (reader.Read()) 
//                            {
//                                int orderid = reader.GetInt32(reader.GetOrdinal("order_id"));
//                                string hashshopdeliverypin = reader.GetString(reader.GetOrdinal("shopdeliveryPin"));
//                                decimal shoplatitude = reader.GetDecimal(reader.GetOrdinal("shoplatitude"));
//                                decimal shoplongitude = reader.GetDecimal(reader.GetOrdinal("shopLongitude"));
//                                string shopphonenumber = reader.GetString(reader.GetOrdinal("shopphonenumber"));
//                                string shopemail = reader.GetString(reader.GetOrdinal("shopemail"));
//                                string clientname = reader.GetString(reader.GetOrdinal("clientname"));
//                                string clientemail = reader.GetString(reader.GetOrdinal("clientemail"));
//                                string clientphonenumber = reader.GetString(reader.GetOrdinal("clientphonenumber"));
//                                var orderdate = reader.GetDateTime(reader.GetOrdinal("order_date"));
//                                var shopname = reader.GetString(reader.GetOrdinal("shopname"));
//                                var shopid = reader.GetInt32(reader.GetOrdinal("shop_id"));
//                                var clientlatitude = reader.GetDecimal(reader.GetOrdinal("clientlatitude"));
//                                var clientlongitude = reader.GetDecimal(reader.GetOrdinal("clintlongitude"));
//                                var ordersfordelivery = new OrderForDelivery
//                                {
//                                    orderid = orderid,
//                                    orderdate=orderdate,
//                                    ShopEmail = shopemail,
//                                    shopname=shopname,
//                                    ShopPhoneNumber = shopphonenumber,
//                                    clientname=clientname,
//                                    clientemail=clientemail,
//                                    clientphonenumber=clientphonenumber,
//                                    shopid=shopid,
//                                    clientlatitude=clientlatitude,
//                                    clientlongitude=clientlongitude,
//                                    shoplatitude=shoplatitude,
//                                    shoplongitude=shoplongitude,
//                                    HashDeliveryShopPin=hashshopdeliverypin
                                    

//                                };
//                                listorder.Add(ordersfordelivery);
//                            }
//                            if (listorder.Count == 0)
//                            {
//                                return new ResultCheckdb<List<OrderForDelivery>>
//                                {
//                                    IsSuccess = true,
//                                    IsFound = false,
//                                };
//                            }
//                            else
//                            {
//                                return new ResultCheckdb<List<OrderForDelivery>>
//                                {
//                                    IsFound = true,
//                                    IsSuccess = true,
//                                    Value = listorder
                                    
//                                };
//                            }
//                        }
//                    }

//                }

//            }
//            catch (Exception ex) 
//            {
//                return new ResultCheckdb<List<OrderForDelivery>>
//                {
//                    IsFound = false,
//                    IsSuccess = false,
//                    Error = ex.Message+"r0"

//                };
//            }
//        }

//        public async Task deliveryreciveorder(RecievefromShopToDeliveryDto recievefromShopToDelivery,int shopid) 
//        {
            
//                using (SqlConnection connect = new SqlConnection(connectionstring))
//                {
//                    await connect.OpenAsync();
//                    //string query = "select shopid,delivery_id,shopdeliveryPin from client order where order_id=@orderid and shopdeliverypin=@shopdeliverypin";
//                    using (SqlCommand cmd = new SqlCommand("recieveorderfromshoptoorder", connect))
//                    {
//                        cmd.CommandType= System.Data.CommandType.StoredProcedure;
//                        cmd.Parameters.AddWithValue("@orderid", recievefromShopToDelivery.orderId);
//                        cmd.Parameters.AddWithValue("@shopid", shopid);
//                        cmd.Parameters.AddWithValue("@shopdeliverypin", recievefromShopToDelivery.password);
//                        cmd.Parameters.AddWithValue("@delivery_id", recievefromShopToDelivery.deliveryid);

//                        SqlParameter iscorrectParam = new SqlParameter("@iscorrectdelivery", SqlDbType.Bit)
//                        {
//                            Direction = ParameterDirection.Output
//                        };
//                        cmd.Parameters.Add(iscorrectParam);
//                        await cmd.ExecuteReaderAsync();
//                        var correctdelivery = Convert.ToBoolean(iscorrectParam.Value);
//                        if (!correctdelivery)
//                        {
//                            throw new Exception("order id or shop delivery pin is incorrect");
//                        }
//                        connect.Close();
                       
//                    }
//                }
            
           
//        }

//        public async Task RecieveOrederFromDeliveryToClient(RecieveFromDeliveryToClientDto recievefromdeliverytoclient)
//        {
            
//                using (SqlConnection connect = new SqlConnection(connectionstring))
//                {
//                    await connect.OpenAsync();
//                    using (SqlCommand command = new SqlCommand("Recieveorderfromdeliverytoclient",connect))
//                    {
//                        command.Parameters.AddWithValue("@orderid",recievefromdeliverytoclient.orderId);
//                        command.Parameters.AddWithValue("@deliveryid", recievefromdeliverytoclient.deliveryid);
//                        command.Parameters.AddWithValue("@clientdeliverypin",recievefromdeliverytoclient.password);

//                        SqlParameter isexecuteparam = new SqlParameter("@isexecute",SqlDbType.Bit)
//                        {
//                            Direction = ParameterDirection.Output,
//                        };
//                        command.Parameters.Add(isexecuteparam);

//                        await command.ExecuteNonQueryAsync();
//                        var isexecuted=Convert.ToBoolean(isexecuteparam.Value);
//                        connect.Close();
//                        if (!isexecuted) 
//                        {
//                            throw new Exception("order id or delivery id or client delivery pin is incorrect");
//                    }
                       
                        
//                    }
//                }
//            }
            
           
//        }

//    }
//}