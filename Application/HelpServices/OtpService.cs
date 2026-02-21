using onlineshopowner_api.Infrastructure.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace onlineshopowner_api.Application.Validatorandclean
{
    public static class OtpService
    {
     
       
        public static string GenerateOtp()
        {
            var rnd = new Random();
            return rnd.Next(100000, 999999).ToString(); // 6 digits
        }

        public static PendingRegistration  VerifyOtp( string otpCode,string email=null,string phonenumber = null)
        {
            if (email != null)
            {
                online_shopEntities1 db = new online_shopEntities1();
                var temp = db.PendingRegistrations.FirstOrDefault(x => x.Email == email && x.OtpCode == otpCode && x.OtpExpiresAt > DateTime.UtcNow);
                if (temp == null)
                {
                    throw new Exception("the is error");
                   
                }
                return temp;
               

            }
            else if (phonenumber != null) 
            {
                online_shopEntities1 db = new online_shopEntities1();
                var temp = db.PendingRegistrations.FirstOrDefault(x => x.Email == email && x.OtpCode == otpCode && x.OtpExpiresAt > DateTime.UtcNow);
                if (temp == null)
                {
                    throw new Exception("the is error");
                }
                return temp;
               
            }
            else
            {
                throw new Exception("there is no phonenumber and email to conntect ");
            }

        }
    }
}