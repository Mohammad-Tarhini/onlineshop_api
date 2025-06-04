using Microsoft.IdentityModel.Tokens;
using onlineshopowner_api.Application.Interfaces.Itoken;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Web;

namespace onlineshopowner_api.Infrastructure.Token
{
    public class JwtTokenGenerator: IjwtTokenGenerator
    {
        private readonly string _secretKey;
        private readonly string _issuer;
        private readonly string _audience;

        public JwtTokenGenerator()
        {
            _secretKey = ConfigurationManager.AppSettings["JwtSecretKey"];
            _issuer = ConfigurationManager.AppSettings["JwtIssuer"];
            _audience = ConfigurationManager.AppSettings["JwtAudience"];
        }
        public string GenerateToken(int userId, string role, int expireMinutes = 60)
        {
            var symmetricKey = Encoding.UTF8.GetBytes(_secretKey);
            var tokenHandler = new JwtSecurityTokenHandler();

            var claims = new[]
            {
            new Claim(ClaimTypes.NameIdentifier, userId.ToString()),
            new Claim(ClaimTypes.Role, role)
        };

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.UtcNow.AddMinutes(expireMinutes),
                Issuer = _issuer,
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(symmetricKey), SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }
    }
    
}
