using onlineshopowner_api.Domain.Entities.Payment;
using onlineshopowner_api.Domain.Interfaces.IRepository;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Drawing.Text;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace onlineshopowner_api.Infrastructure.Repositories
{
    public class PaymentRepository: IPaymentRepository
    {
        private string connectionString { get; set; }
        public PaymentRepository()
        {
            connectionString = ConfigurationManager.ConnectionStrings["online_shopAdo"].ConnectionString;
        }

        public async Task RegisterPayIn(PayIn payIn)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                await connection.OpenAsync();
                string query = "INSERT INTO PayIn (OrderId, Amount, PaymentMethod, PaymentDate) VALUES (@OrderId, @Amount, @PaymentMethod, @PaymentDate)";
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@OrderId", payIn.OrderId);
                    command.Parameters.AddWithValue("@Amount", payIn.Amount);
                    command.Parameters.AddWithValue("@PaymentMethod", payIn.PaymentMethod);
                    command.Parameters.AddWithValue("@PaymentDate", payIn.PaymentDate);
                    await command.ExecuteNonQueryAsync();
                }
                connection.Close();


            }
        }

        public async Task RegisterInternalTransaction(InternalTransaction internalTransaction)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                await connection.OpenAsync();
                string query = "INSERT INTO InternalTransaction (UserId, OrderId, Amount,Description, TransactionDate) VALUES (@userId, @orderId, @Amount,@description, @TransactionDate)";
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@userId", internalTransaction.userId);
                    command.Parameters.AddWithValue("@orderId", internalTransaction.orderId);
                    command.Parameters.AddWithValue("@Amount", internalTransaction.Amount);
                    command.Parameters.AddWithValue("@description", internalTransaction.description);
                    command.Parameters.AddWithValue("@TransactionDate", internalTransaction.TransactionDate);
                    await command.ExecuteNonQueryAsync();
                }
                connection.Close();
            }
        }
        public async Task RegisterPayOut(PayOut payOut)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                await connection.OpenAsync();
                string query = "INSERT INTO PayOut (userId,Amount, Method,status, createdAt) VALUES (@userId,@Amount, @PaymentMethod, @PaymentDateTime)";
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@userId", payOut.userId);
                    command.Parameters.AddWithValue("@PaymentMethod", payOut.PaymentMethod);
                    command.Parameters.AddWithValue("@PaymentDateTime", payOut.PaymentDate);
                    command.Parameters.AddWithValue("@Amount", payOut.Amount);
                    await command.ExecuteNonQueryAsync();
                }
                connection.Close();

            }
        }
    }
}