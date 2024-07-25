using Microsoft.Extensions.Caching.Memory;
using WebAPI.Application.Interfaces;

namespace WebAPI.Infrastructure;
public class MemoryCacheService : ICachingService
{
    private readonly IMemoryCache _memoryCache;

    public MemoryCacheService(IMemoryCache memoryCache)
    {
        _memoryCache = memoryCache;
    }

    public Task SetCacheAsync<T>(string key, T data, TimeSpan expiration)
    {
        _memoryCache.Set(key, data, new MemoryCacheEntryOptions
        {
            AbsoluteExpirationRelativeToNow = expiration
        });
        return Task.CompletedTask;
    }

    public ValueTask<T> GetCacheAsync<T>(string key)
    {
        var data = _memoryCache.TryGetValue(key, out T cachedData) ? cachedData : default;
        return new ValueTask<T>(data);
    }

}

