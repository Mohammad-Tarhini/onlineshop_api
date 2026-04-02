using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using onlineshopowner_api.Application.Dtos;
using onlineshopowner_api.Application.Validatorandclean;

namespace onlineshopowner_api.Infrastructure.Token
{



    public static class VerificationTokenLink
    {
        private const string SECRET_KEY = "VERY_LONG_RANDOM_SECRET_KEY";

        public static string GenerateToken(
            PersonDto person,
            string role
        )
        {
            string passwordHash = HashingPassword.HashPassword(person.Password);

            string expires =
                DateTime.UtcNow.AddMinutes(15)
                .ToString("yyyyMMddHHmmss");

            // payload = DATA (NO signature yet)
            string payload =
                $"{person.FirstName}|{person.LastName}|{person.Email}|{person.PhoneNumber}|{passwordHash}|{person.Sex}|{role}|{expires}";

            string signature = Sign(payload);

            string fullToken = payload + "|" + signature;

            return Convert.ToBase64String(
                Encoding.UTF8.GetBytes(fullToken));
        }

        public static bool ValidateToken(
            string token,
            out PersonDto person,
            out string role
        )
        {
            person = null;
            role = null;

            string decoded;
            try
            {
                decoded = Encoding.UTF8.GetString(
                    Convert.FromBase64String(token));
            }
            catch { return false; }

            var parts = decoded.Split('|');
            if (parts.Length != 9) return false;

            var payload =
                string.Join("|", parts.Take(8));
            var sentSignature = parts[8];

            if (Sign(payload) != sentSignature)
                return false;

            var expires = DateTime.ParseExact(
                parts[7], "yyyyMMddHHmmss", null);

            if (DateTime.UtcNow > expires)
                return false;

            person = new PersonDto
            {
                FirstName = parts[0],
                LastName = parts[1],
                Email = parts[2],
                PhoneNumber = parts[3],
                Password = parts[4], // already HASHED
                Sex = parts[5]
            };

            role = parts[6];

            return true;
        }

        private static string Sign(string data)
        {
            using (var hmac = new HMACSHA256(
                Encoding.UTF8.GetBytes(SECRET_KEY)))
            {
                return Convert.ToBase64String(
                    hmac.ComputeHash(
                        Encoding.UTF8.GetBytes(data)));
            }
        }
    }

    //public class VerificationToken
    //{
    //    private const string SECRET_KEY = "CHANGE_THIS_TO_A_LONG_RANDOM_SECRET";

    //    public static string Generate(string email, string passwordHash)
    //    {
    //        var expires = DateTime.UtcNow.AddHours(24).ToString("yyyyMMddHHmmss");

    //        var payload = $"{email}|{passwordHash}|{expires}";
    //        var signature = Sign(payload);

    //        var token = $"{payload}|{signature}";
    //        return Convert.ToBase64String(Encoding.UTF8.GetBytes(token));
    //    }

    //    public static bool Validate(
    //        string token,
    //        out string email,
    //        out string passwordHash)
    //    {
    //        email = null;
    //        passwordHash = null;

    //        string decoded;
    //        try
    //        {
    //            decoded = Encoding.UTF8.GetString(
    //                Convert.FromBase64String(token)
    //            );
    //        }
    //        catch { return false; }

    //        var parts = decoded.Split('|');
    //        if (parts.Length != 4) return false;

    //        email = parts[0];
    //        passwordHash = parts[1];
    //        var expires = DateTime.ParseExact(parts[2], "yyyyMMddHHmmss", null);
    //        var sentSignature = parts[3];

    //        if (DateTime.UtcNow > expires)
    //            return false;

    //        var payload = $"{email}|{passwordHash}|{parts[2]}";
    //        var expectedSignature = Sign(payload);

    //        return sentSignature == expectedSignature;
    //    }

    //    private static string Sign(string data)
    //    {
    //        using (var hmac = new HMACSHA256(Encoding.UTF8.GetBytes(SECRET_KEY)))
    //        {
    //            return Convert.ToBase64String(
    //                hmac.ComputeHash(Encoding.UTF8.GetBytes(data))
    //            );
    //        }
    //    }
    //}
}