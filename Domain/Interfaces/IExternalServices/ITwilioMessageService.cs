using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace onlineshopowner_api.Domain.Interfaces.IExternalServices
{
    public interface ITwilioMessageService
    {
        Task SendSmsAsync(string toPhoneNumber, string message);
    }
}