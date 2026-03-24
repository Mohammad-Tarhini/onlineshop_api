using onlineshopowner_api.Domain.Interfaces.IExternalServices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using static onlineshopowner_api.Infrastructure.ExternalServices.Payment.FakeGateWayService;
using static System.Net.WebRequestMethods;

namespace onlineshopowner_api.Infrastructure.ExternalServices.Payment
{
    public class FakeGateWayService: IFakeGatewayService
    {
       
            private static readonly Dictionary<string, GatewayPayment> _storage
                = new Dictionary<string, GatewayPayment>();

            public  Task<CheckoutSessionResponse> CreateSessionAsync(CheckoutSessionRequest request)
            {
                var sessionId = Guid.NewGuid().ToString();

                _storage[sessionId] = new GatewayPayment
                {
                    SessionId = sessionId,
                    OrderId = request.OrderId,
                    Amount = request.Amount,
                    Status = "Pending",
                    WebhookUrl = "https://localhost:44364/api/gateway/webhook",
                };

                return Task.FromResult(new CheckoutSessionResponse
                {
                    SessionId = sessionId,
                    CheckoutUrl = $"https://localhost:44364/api/gateway/paymentProcess?sessionId={sessionId}"
                });
            }

            public async Task<GatewayPayment> ProcessPaymentAsync(string sessionId, string cardNumber)
            {
                if (!_storage.ContainsKey(sessionId))
                    return null;

                var payment = _storage[sessionId];

            //payment.Status = cardNumber.StartsWith("4")
            //    ? "Paid"
            //    : "Failed";
              payment.Status = "paid";
            return payment;
            //await SendWebhookAsync(payment);

            }

        private async Task SendWebhookAsync(GatewayPayment payment)
        {
            using (var client = new HttpClient())
            {
                if (string.IsNullOrEmpty(payment.WebhookUrl))
                    throw new Exception("Webhook URL is null");

                var payload = new
                {
                    sessionId = payment.SessionId,
                    orderId = payment.OrderId,
                    amount = payment.Amount,
                    status = payment.Status
                };

                try
                {
                    var response = await client.PostAsJsonAsync(payment.WebhookUrl, payload);

                    var responseContent = await response.Content.ReadAsStringAsync();

                    Console.WriteLine($"Webhook response: {response.StatusCode}");
                    Console.WriteLine(responseContent);

                    if (!response.IsSuccessStatusCode)
                    {
                        throw new Exception($"Webhook failed: {responseContent}");
                    }
                }
                catch (Exception ex)
                {
                    //Console.WriteLine($"Webhook error: {ex.Message}");
                    throw new Exception(ex.ToString());
                }
            }
        }
    }

    }
