using onlineshopowner_api.Application.Interfaces.Iservices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading;
using System.Web;

namespace onlineshopowner_api.Application.Validatorandclean
{
    public class UserContextServices:IUserContextService
    {

        public int GetUserId()
        {
            var principal = Thread.CurrentPrincipal as ClaimsPrincipal;

            if (principal == null)
                throw new UnauthorizedAccessException("User is not authenticated.");

            var claim = principal.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier || c.Type == "sub" || c.Type == "nameid");

            if (claim == null)
                throw new UnauthorizedAccessException("User ID not found in token.");

            if (int.TryParse(claim.Value, out int userId))
                return userId;
            else
                throw new UnauthorizedAccessException("Invalid user ID in token.");

        }

        public string GetUserRole()
        {
            var principal = Thread.CurrentPrincipal as ClaimsPrincipal;

            var claim = principal?.Claims.FirstOrDefault(c =>
                c.Type == ClaimTypes.Role || c.Type == "role");

            if (claim == null)
                throw new UnauthorizedAccessException("User role not found in token.");

            return claim.Value;
        }
    }
}
