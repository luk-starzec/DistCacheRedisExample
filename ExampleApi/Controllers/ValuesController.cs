using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Distributed;
using System.Text;

namespace ExampleApi.Controllers;

[Route("api/[controller]")]
[ApiController]
public class ValuesController : ControllerBase
{
    private readonly string _cacheKey = "TestValue";
    private readonly IDistributedCache _cache;

    public ValuesController(IDistributedCache cache)
    {
        _cache = cache;
    }

    [HttpGet]
    public async Task<string> Get()
    {
        return await GetValue();
    }

    [HttpPost]
    public async Task Post([FromBody] DateTime value)
    {
        await CacheValue(_cacheKey, value.ToString());
    }

    [HttpDelete()]
    public async Task Delete()
    {
        await _cache.RemoveAsync(_cacheKey);
    }

    private async Task<string> GetValue()
    {
        var encodedCachedTime = await _cache.GetAsync(_cacheKey);

        if (encodedCachedTime != null)
        {
            var cachedTime = Encoding.UTF8.GetString(encodedCachedTime);
            return cachedTime.ToString();
        }

        var currentTime = DateTime.Now;
        await CacheValue(_cacheKey, currentTime.ToString());

        return $"{currentTime} (No cache)";
    }

    private async Task CacheValue(string key, string value)
    {
        byte[] encodedCurrentTime = System.Text.Encoding.UTF8.GetBytes(value);
        var options = new DistributedCacheEntryOptions().SetSlidingExpiration(TimeSpan.FromSeconds(10));

        await _cache.SetAsync(key, encodedCurrentTime, options);
    }
}
