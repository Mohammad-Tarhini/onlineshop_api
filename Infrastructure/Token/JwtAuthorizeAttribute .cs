using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading;
using System.Web;
using System.Web.Http;
using System.Web.Http.Controllers;
using Microsoft.IdentityModel.Tokens;
using System.Configuration;
using System.Linq;

public class JwtAuthorizeAttribute : AuthorizeAttribute
{
    protected override bool IsAuthorized(HttpActionContext actionContext)
    {
        var tokenHeader = actionContext.Request.Headers.Authorization;
        if (tokenHeader == null || tokenHeader.Scheme != "Bearer")
            return false;

        var token = tokenHeader.Parameter;
        var tokenHandler = new JwtSecurityTokenHandler();
        var key = Encoding.UTF8.GetBytes(ConfigurationManager.AppSettings["JwtSecretKey"]);

        var parameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidIssuer = ConfigurationManager.AppSettings["JwtIssuer"],
            ValidateAudience = false,
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(key),
            ValidateLifetime = true,
            ClockSkew = TimeSpan.Zero
        };
        

        try
        {
            SecurityToken validatedToken;
            var principal = tokenHandler.ValidateToken(token, parameters, out validatedToken);

            // Set the user for Web API and Thread
            Thread.CurrentPrincipal = principal;
            if (HttpContext.Current != null)
                HttpContext.Current.User = principal;
            if (!string.IsNullOrWhiteSpace(Roles))
            {
                // Get all roles from the token
                var tokenRoles = principal.Claims
                    .Where(c => c.Type == ClaimTypes.Role || c.Type == "role")
                    .Select(c => c.Value)
                    .ToList();

                // Split allowed roles from attribute
                var allowedRoles = Roles.Split(',').Select(r => r.Trim()).ToList();

                // Check if any role from token is in allowed roles
                return tokenRoles.Any(role => allowedRoles.Contains(role, StringComparer.OrdinalIgnoreCase));
            }

            return true;
        }
        catch
        {
            return false;
        }
    }
}
