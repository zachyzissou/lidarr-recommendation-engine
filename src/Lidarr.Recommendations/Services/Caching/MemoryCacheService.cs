using Microsoft.Extensions.Caching.Memory;

namespace Lidarr.Recommendations.Services.Caching;

public sealed class MemoryCacheService : IMemoryCacheService, IDisposable
{
    private readonly MemoryCache _cache = new(new MemoryCacheOptions());

    public ValueTask<T> GetOrCreateAsync<T>(string key, Func<CancellationToken, Task<T>> factory, TimeSpan ttl, CancellationToken ct)
        => new(GetOrCreateInternalAsync(key, factory, ttl, ct));

    private async Task<T> GetOrCreateInternalAsync<T>(string key, Func<CancellationToken, Task<T>> factory, TimeSpan ttl, CancellationToken ct)
    {
        if (_cache.TryGetValue(key, out var existing) && existing is T t) return t;

        var value = await factory(ct);
        _cache.Set(key, value!, new MemoryCacheEntryOptions
        {
            AbsoluteExpirationRelativeToNow = ttl
        });
        return (T)value!;
    }

    public void Remove(string key) => _cache.Remove(key);

    public void Dispose() => _cache.Dispose();
}
