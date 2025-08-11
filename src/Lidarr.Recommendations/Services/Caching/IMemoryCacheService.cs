namespace Lidarr.Recommendations.Services.Caching;

public interface IMemoryCacheService
{
    ValueTask<T> GetOrCreateAsync<T>(string key, Func<CancellationToken, Task<T>> factory, TimeSpan ttl, CancellationToken ct);
    void Remove(string key);
}
