using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;
using onlineshopowner_api.Domain.Interfaces.IExternalServices;
namespace onlineshopowner_api.Infrastructure.ExternalServices.comunicate
{
    public class EmailService:IEmailService
    {
        public  async Task SendEmailAsync(string to, string subject, string body)
        {
            try
            {
                var addTo=new MailAddress(to);
                var message = new MailMessage();
                message.To.Add(to);
                message.From=new MailAddress("tarhinimohammadbaker@gmail.com");
                message.Subject = subject;
                message.Body = body;
                message.IsBodyHtml = true;

                using (var smtp = new SmtpClient())
                {
                    await smtp.SendMailAsync(message);
                }
                //var sent =await EmailService.sendEmailAsync(to, subject, body);
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


//public async Task SendEmailAsync(string to, string subject, string body)
//{
//    try
//    {
//        var message = new MailMessage
//        {
//            From = new MailAddress("tarhinimohammadbaker@gmail.com", "Online Shop"),
//            Subject = subject,
//            Body = body,
//            IsBodyHtml = true
//        };

//        message.To.Add(to);

//        using (var smtp = new SmtpClient())
//        {
//            await smtp.SendMailAsync(message);
//        }
//    }
//    catch (SmtpException ex)
//    {
//        // Log ex.Message, ex.StatusCode, stack trace
//        throw;
//    }
//}
