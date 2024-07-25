namespace WebAPI.Application.Interfaces;
public interface ICachingService
{
    Task SetCacheAsync<T>(string key, T data, TimeSpan expiration);
    ValueTask<T> GetCacheAsync<T>(string key);
}

