using System.Threading.Tasks;

namespace RedisCacheAsService.Interfaces
{
    public interface ICacheService
    {
        Task SetStringAsync(string key, string value);
        Task<string> GetStringAsync(string key);
        Task SetObjectAsync<T>(string key, T value) where T : class;
        Task<T> GetObjectAsync<T>(string key) where T : class;
        Task<bool> RemoveAsync(string key);
        Task<bool> IsInCache(string key);
    }
}