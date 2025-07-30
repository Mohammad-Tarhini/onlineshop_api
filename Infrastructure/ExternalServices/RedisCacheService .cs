using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using onlineshopowner_api.Domain.Interfaces.IExternalServices;
using System.Text.Json;

namespace onlineshopowner_api.Infrastructure.ExternalServices
{

    namespace onlineshopowner_api.Infrastructure.ExternalServices
    {
        public class RedisCacheService : IRedisCacheService
        {
            private readonly IDatabase _db;
            private readonly JsonSerializerOptions _jsonOptions;

            public RedisCacheService()
            {
                var config = new ConfigurationOptions
                {
                    EndPoints = { "192.168.1.200:6379" },
                    AbortOnConnectFail = false,
                };
                var redis = ConnectionMultiplexer.Connect(config); // your Ubuntu VM IP
                _db = redis.GetDatabase();
                _jsonOptions = new JsonSerializerOptions
                {
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                    WriteIndented = false,
                };
                
            }

            public async Task SetAsync(string key, string value, TimeSpan? expiry = null)
            {
                await _db.StringSetAsync(key, value);
            }

            public async Task<string> GetAsync(string key)
            {
                return await _db.StringGetAsync(key);
            }
            // For object values (serialized to JSON)
            public async Task SetObjectAsync<T>(string key, T value, TimeSpan? expiry = null)
            {
                var json = JsonSerializer.Serialize(value, _jsonOptions);
                await SetAsync(key, json, expiry);
            }

            public async Task<T> GetObjectAsync<T>(string key)
            {
                var json = await GetAsync(key);
                return json == null ? default : JsonSerializer.Deserialize<T>(json, _jsonOptions);
            }


            public async Task<bool> DeleteAsync(string key)
            {
                return await _db.KeyDeleteAsync(key);
            }

            public async Task<bool> AddToSortedSetAsync(string sortedSetKey, string member, double score)
            {
                return await _db.SortedSetAddAsync(sortedSetKey, member, score);
            }

            public async Task<RedisValue[]> GetSortedSetRangeByRankAsync(string sortedSetKey, int start, int stop)
            {
                return await _db.SortedSetRangeByRankAsync(sortedSetKey, start, stop);
            }
            // Sorted Set Methods
            public async Task<bool> RemoveFromSortedSetAsync(string sortedSetKey, string member)
            {
                return await _db.SortedSetRemoveAsync(sortedSetKey, member);
            }

            public async Task<long> GetSortedSetLengthAsync(string sortedSetKey)
            {
                return await _db.SortedSetLengthAsync(sortedSetKey);
            }

            public async Task<bool> SortedSetContainsAsync(string sortedSetKey, string member)
            {
                var score = await _db.SortedSetScoreAsync(sortedSetKey, member);
                return score.HasValue;
            }
        }
    }
}