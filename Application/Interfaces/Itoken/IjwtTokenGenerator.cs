using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace onlineshopowner_api.Application.Interfaces.Itoken
{
    public interface IjwtTokenGenerator
    {
        string GenerateToken(int personId, string role, int expireMinutes);
    }
}
