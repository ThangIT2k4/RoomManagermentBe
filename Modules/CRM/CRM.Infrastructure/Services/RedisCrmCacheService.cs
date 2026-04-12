using System.Text.Json;
using CRM.Application.Services;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;

namespace CRM.Infrastructure.Services;

public sealed class RedisCrmCacheService(IDistributedCache cache, ILogger<RedisCrmCacheService> logger) : ICrmCacheService
{
    public async Task<T?> GetAsync<T>(string key, CancellationToken cancellationToken = default)
    {
        try
        {
            var value = await cache.GetStringAsync(key, cancellationToken);
            return string.IsNullOrWhiteSpace(value) ? default : JsonSerializer.Deserialize<T>(value);
        }
        catch (Exception ex)
        {
            logger.LogWarning(ex, "Redis cache get thất bại với khóa {Key}. Chuyển sang đọc DB.", key);
            return default;
        }
    }

    public async Task SetAsync<T>(string key, T value, TimeSpan ttl, CancellationToken cancellationToken = default)
    {
        try
        {
            var payload = JsonSerializer.Serialize(value);
            await cache.SetStringAsync(key, payload, new DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = ttl
            }, cancellationToken);
        }
        catch (Exception ex)
        {
            logger.LogWarning(ex, "Redis cache set thất bại với khóa {Key}.", key);
        }
    }

    public async Task RemoveAsync(string key, CancellationToken cancellationToken = default)
    {
        try
        {
            await cache.RemoveAsync(key, cancellationToken);
        }
        catch (Exception ex)
        {
            logger.LogWarning(ex, "Redis cache remove thất bại với khóa {Key}.", key);
        }
    }
}
