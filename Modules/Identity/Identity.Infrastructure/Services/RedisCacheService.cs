using System.Collections.Concurrent;
using System.Text.Json;
using Identity.Application.Services;
using StackExchange.Redis;

namespace Identity.Infrastructure.Services;

public class RedisCacheService : ICacheService
{
    private readonly IConnectionMultiplexer _redis;
    private readonly IDatabase _database;

    public RedisCacheService(IConnectionMultiplexer redis)
    {
        _redis = redis;
        _database = _redis.GetDatabase();
    }
    
    public async Task<T?> GetAsync<T>(string key)
    {
        var value = await _database.StringGetAsync(key);

        if (value.IsNullOrEmpty)
            return default;
        return JsonSerializer.Deserialize<T>(value);
    }

    public async Task SetAsync<T>(string key, T value, TimeSpan? expiry = null)
    {
        var json = JsonSerializer.Serialize(value);
    
        var expiration = expiry.HasValue 
            ? new Expiration(expiry.Value) 
            : Expiration.Default;
    
        await _database.StringSetAsync(key, json, expiration);
    }


    public async Task RemoveAsync(string key)
    {
        await _database.KeyDeleteAsync(key);
    }

    public async Task RemoveByPatternAsync(string pattern)
    {
        var endpoints = _redis.GetEndPoints();
        var server = _redis.GetServer(endpoints.First());
    
        var keys = server.Keys(pattern: pattern);
    
        foreach (var key in keys)
        {
            await _database.KeyDeleteAsync(key);
        }
    }
}