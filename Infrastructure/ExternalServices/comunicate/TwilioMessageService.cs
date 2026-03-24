using AutoMapper.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Twilio.Types;
using Twilio;
using System.Configuration;
using Twilio.Rest.Api.V2010.Account;
using System.Threading.Tasks;
using onlineshopowner_api.Domain.Interfaces.IExternalServices;

namespace onlineshopowner_api.Infrastructure.ExternalServices.comunicate
{
    public class TwilioMessageService:ITwilioMessageService
    {
        
    private readonly string accountSid;
        private readonly string authToken;
        private readonly string fromPhone;

        public  TwilioMessageService()
        {
             accountSid = ConfigurationManager.AppSettings["TwilioAccountSid"];
             authToken = ConfigurationManager.AppSettings["TwilioAuthToken"];
             fromPhone = ConfigurationManager.AppSettings["TwilioPhoneNumber"];
            TwilioClient.Init(accountSid, authToken);
        }

        public   async Task SendSmsAsync(string toPhoneNumber, string message)
        {
            try
            {
                var result = await MessageResource.CreateAsync(
                    to: new PhoneNumber(toPhoneNumber),
                    from: new PhoneNumber(fromPhone),
                    body: message
                );
                if(result.ErrorCode != null)
                {
                    throw new Exception(result.ErrorMessage);
                }

                // Check status or SID to confirm success
               // return result.ErrorCode == null;
            }
            catch (Exception ex)
            {
                // Optionally log the exception
                // e.g., Console.WriteLine(ex.Message);
                throw;
            }
        }
    }
}