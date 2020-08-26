using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using RedisCacheAsService.Helpers;
using RedisCacheAsService.Interfaces;
using StackExchange.Redis;

namespace RedisCacheAsService.Services
{
    public class RedisCacheService : ICacheService
    {
        private readonly string _redisHost;
        private readonly int _redisPort;
        private ConnectionMultiplexer _connectionMultiplexer;
        private IDatabase _db;
        
        public RedisCacheService(IConfiguration config)
        {
            _redisHost = RedisCacheSettings.Host;
            _redisPort = RedisCacheSettings.Port;
        }

        public void Connect()
        {
            try
            {
                Console.WriteLine("Connecting to Redis...");
                var configString = $"{_redisHost}:{_redisPort}, connectRetry=5, connectTimeout=5000";
                _connectionMultiplexer = ConnectionMultiplexer.Connect(configString);
                Console.WriteLine("Connected to Redis successfully :)");
                GetDb();
            }
            catch (RedisConnectionException err)
            {
                Console.WriteLine("Redis Connection Failed... " + err);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Exception occured in Redis connection... " + ex);
            }
        }

        public async Task SetStringAsync(string key, string value)
        {
            await _db.StringSetAsync(key, value);
        }

        public async Task<string> GetStringAsync(string key)
        {
            var res = await _db.StringGetAsync(key);
            return res;
        }

        public async Task SetObjectAsync<T>(string key, T value) where T : class
        {
            var json = JsonConvert.SerializeObject(value);
            await _db.StringSetAsync(key, json);
        }

        public async Task<T> GetObjectAsync<T>(string key) where T : class
        {
            var json = await _db.StringGetAsync(key);
            if (!json.IsNullOrEmpty)
                return JsonConvert.DeserializeObject<T>(json);
            return null;
        }
        
        public async Task<bool> RemoveAsync(string key)
        {
            var res = await _db.KeyDeleteAsync(key);
            return res;
        }

        public async Task<bool> IsInCache(string key)
        {
            var res = await _db.StringGetAsync(key);
            return !res.IsNullOrEmpty;
        }



        #region Private Methods

        private void GetDb()
        {
            _db = _connectionMultiplexer.GetDatabase(1);
            Console.WriteLine("Got DB: " + _db);
        }

        private static bool IsValidJson(string strInput)
        {
            if (string.IsNullOrWhiteSpace(strInput)) { return false; }
            strInput = strInput.Trim();
            if ((strInput.StartsWith("{") && strInput.EndsWith("}")) || //For object
                (strInput.StartsWith("[") && strInput.EndsWith("]"))) //For array
            {
                try
                {
                    var obj = JToken.Parse(strInput);
                    return true;
                }
                catch (JsonReaderException jex)
                {
                    //Exception in parsing json
                    Console.WriteLine(jex.Message);
                    return false;
                }
                catch (Exception ex) //some other exception
                {
                    Console.WriteLine(ex.ToString());
                    return false;
                }
            }
            return false;
        }

        #endregion

    }
}