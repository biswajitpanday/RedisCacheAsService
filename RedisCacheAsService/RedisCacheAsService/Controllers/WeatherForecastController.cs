using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using RedisCacheAsService.Models;
using RedisCacheAsService.Services;

namespace RedisCacheAsService.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class WeatherForecastController : ControllerBase
    {
        private readonly RedisCacheService _cacheService;
        
        private static readonly string[] Summaries = new[]
        {
            "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
        };

        private readonly ILogger<WeatherForecastController> _logger;

        public WeatherForecastController(ILogger<WeatherForecastController> logger, RedisCacheService cacheService)
        {
            _logger = logger;
            _cacheService = cacheService;
        }

        [HttpGet]
        public IEnumerable<WeatherForecast> Get()
        {
            var rng = new Random();
            return Enumerable.Range(1, 5).Select(index => new WeatherForecast
            {
                Date = DateTime.Now.AddDays(index),
                TemperatureC = rng.Next(-20, 55),
                Summary = Summaries[rng.Next(Summaries.Length)]
            })
            .ToArray();
        }

        [HttpPost("StoreInCache")]
        public async Task StoreInCache(CacheRequestModel cacheRequestModel)
        {
            await _cacheService.Set(cacheRequestModel.Key, cacheRequestModel.Value);
        }

        [HttpGet("GetFromCache")]
        public async Task<IActionResult> GetFromCache(string key)
        {
            var data = await _cacheService.Get(key);
            return Ok(data);
        }
    }
}
