using System;
using System.Threading.Tasks;

namespace RedisCacheAsService.Interfaces
{
    public interface ICacheService
    {
        //Task Get<T>(string key);
        //Task Set<T>(string key, T value);
        //Task Set<T>(string key, T value, TimeSpan timeout);
        //Task<bool> Remove(string key);
        //Task<bool> IsInCache(string key);


        Task Set(string key, string value);
        Task Set(string key, object value);
        Task<string> Get(string key);
        Task<bool> Remove(string key);
        Task<bool> IsInCache(string key);
    }
}