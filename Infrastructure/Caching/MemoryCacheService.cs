using Infrastructure.Abstract;
using Microsoft.Extensions.Caching.Memory;

namespace Infrastructure.Caching;

public class MemoryCacheService(IMemoryCache memoryCache) : ICacheService
{
    private readonly IMemoryCache _memoryCache = memoryCache;

    public T Get<T>(string key)
    {
        return _memoryCache.TryGetValue(key, out var value) ? (T)value : default;
    }

    public void Set<T>(string key, T value, TimeSpan duration)
    {
        _memoryCache.Set(key, value, duration);
    }

    public void Remove(string key)
    {
        _memoryCache.Remove(key);
    }

    public async Task<T> GetOrAddAsync<T>(string key, Func<Task<T>> factory, TimeSpan duration)
    {
        if (_memoryCache.TryGetValue<T>(key, out var cached))
            return cached;

        var result = await factory();
        _memoryCache.Set(key, result, duration);
        return result;
    }
}