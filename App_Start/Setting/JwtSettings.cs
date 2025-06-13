using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace onlineshopowner_api.App_Start.Setting
{
    public class JwtSettings
    {
        public string SecretKey { get; set; }
        public string Issuer {  get; set; }
        public string Audience { get; set; }
    }
}