using onlineshopowner_api.Infrastructure.ExternalServices.Payment;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace onlineshopowner_api.Domain.Interfaces.IExternalServices
{
    public  interface IFakeGatewayService
    {
        Task<CheckoutSessionResponse> CreateSessionAsync(CheckoutSessionRequest request);
        Task<GatewayPayment> ProcessPaymentAsync(string sessionId, string cardNumber);
    }
}
