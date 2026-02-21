using onlineshopowner_api.Domain.Interfaces.IExternalServices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using static onlineshopowner_api.Infrastructure.ExternalServices.Payment.FakeGateWayService;

namespace onlineshopowner_api.Infrastructure.ExternalServices.Payment
{
    public class FakeGateWayService: IFakeGatewayService
    {
       
            private static readonly Dictionary<string, GatewayPayment> _storage
                = new Dictionary<string, GatewayPayment>();

            public Task<CheckoutSessionResponse> CreateSessionAsync(CheckoutSessionRequest request)
            {
                var sessionId = Guid.NewGuid().ToString();

                _storage[sessionId] = new GatewayPayment
                {
                    SessionId = sessionId,
                    OrderId = request.OrderId,
                    Amount = request.Amount,
                    Status = "Pending",
                    WebhookUrl = request.WebhookUrl
                };

                return Task.FromResult(new CheckoutSessionResponse
                {
                    SessionId = sessionId,
                    CheckoutUrl = $"https://localhost:5001/gateway/checkout/{sessionId}"
                });
            }

            public async Task ProcessPaymentAsync(string sessionId, string cardNumber)
            {
                if (!_storage.ContainsKey(sessionId))
                    return;

                var payment = _storage[sessionId];

                payment.Status = cardNumber.StartsWith("4")
                    ? "Paid"
                    : "Failed";

                await SendWebhookAsync(payment);
            }

            private async Task SendWebhookAsync(GatewayPayment payment)
            {
            using (var client = new HttpClient())
            {

                var payload = new
                {
                    sessionId = payment.SessionId,
                    orderId = payment.OrderId,
                    amount = payment.Amount,
                    status = payment.Status
                };

                await client.PostAsJsonAsync(payment.WebhookUrl, payload);
            }
            }
        }

    }
