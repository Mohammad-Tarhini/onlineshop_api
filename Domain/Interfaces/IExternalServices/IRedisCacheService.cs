using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace onlineshopowner_api.Domain.Interfaces.IExternalServices
{
    public  interface IRedisCacheService
    {
        Task SetAsync(string key, string value, TimeSpan? expiry = null);
        Task<string> GetAsync(string key);
         Task<T> GetObjectAsync<T>(string key);
        Task SetObjectAsync<T>(string key, T value, TimeSpan? expiry = null);
        Task<bool> DeleteAsync(string key);
    }
}
