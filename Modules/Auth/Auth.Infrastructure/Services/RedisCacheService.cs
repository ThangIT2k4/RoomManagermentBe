using System.Text.Json;
using Auth.Application.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using StackExchange.Redis;

namespace Auth.Infrastructure.Services;

public sealed class RedisCacheService : ICacheService
{
    private readonly IConfiguration _configuration;
    private readonly ILogger<RedisCacheService> _logger;
    private readonly SemaphoreSlim _lock = new(1, 1);
    private IConnectionMultiplexer? _redis;
    private IDatabase? _database;

    public RedisCacheService(IConfiguration configuration, ILogger<RedisCacheService> logger)
    {
        _configuration = configuration;
        _logger = logger;
    }

    public async Task<T?> GetAsync<T>(string key, CancellationToken cancellationToken = default)
    {
        try
        {
            var database = await GetDatabaseAsync(cancellationToken);
            if (database is null)
            {
                return default;
            }

            var value = await database.StringGetAsync(key);
            if (value.IsNullOrEmpty)
            {
                return default;
            }

            return JsonSerializer.Deserialize<T>(value!);
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Redis get thất bại với khóa {CacheKey}.", key);
            return default;
        }
    }

    public async Task SetAsync<T>(string key, T value, TimeSpan? expiry = null, CancellationToken cancellationToken = default)
    {
        try
        {
            var database = await GetDatabaseAsync(cancellationToken);
            if (database is null)
            {
                return;
            }

            var json = JsonSerializer.Serialize(value);
            if (expiry.HasValue)
            {
                await database.StringSetAsync(key, json, expiry.Value);
            }
            else
            {
                await database.StringSetAsync(key, json);
            }
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Redis set thất bại với khóa {CacheKey}.", key);
        }
    }

    public async Task RemoveAsync(string key, CancellationToken cancellationToken = default)
    {
        try
        {
            var database = await GetDatabaseAsync(cancellationToken);
            if (database is null)
            {
                return;
            }

            await database.KeyDeleteAsync(key);
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Redis remove thất bại với khóa {CacheKey}.", key);
        }
    }

    public async Task RemoveByPatternAsync(string pattern, CancellationToken cancellationToken = default)
    {
        try
        {
            var database = await GetDatabaseAsync(cancellationToken);
            if (database is null)
            {
                return;
            }

            var endpoints = _redis?.GetEndPoints() ?? [];
            if (endpoints.Length == 0)
            {
                return;
            }

            var server = _redis!.GetServer(endpoints[0]);
            foreach (var key in server.Keys(pattern: pattern))
            {
                await database.KeyDeleteAsync(key);
            }
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Redis xóa theo mẫu thất bại với pattern {Pattern}.", pattern);
        }
    }

    private async Task<IDatabase?> GetDatabaseAsync(CancellationToken cancellationToken)
    {
        if (_database is not null)
        {
            return _database;
        }

        await _lock.WaitAsync(cancellationToken);
        try
        {
            if (_database is not null)
            {
                return _database;
            }

            var connectionString = BuildConnectionString();
            if (string.IsNullOrWhiteSpace(connectionString))
            {
                return null;
            }

            try
            {
                _redis = await ConnectionMultiplexer.ConnectAsync(connectionString);
                _database = _redis.GetDatabase();
                return _database;
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Kết nối Redis thất bại. Cache sẽ hoạt động ở chế độ no-op.");
                _redis = null;
                _database = null;
                return null;
            }
        }
        finally
        {
            _lock.Release();
        }
    }

    private string? BuildConnectionString()
    {
        var configurationConnectionString = _configuration["Redis:Configuration"];
        if (!string.IsNullOrWhiteSpace(configurationConnectionString))
        {
            return configurationConnectionString;
        }

        var explicitConnectionString = _configuration["Redis:ConnectionString"];
        if (!string.IsNullOrWhiteSpace(explicitConnectionString))
        {
            return explicitConnectionString;
        }

        var fallbackConnectionString = _configuration.GetConnectionString("Redis");
        if (!string.IsNullOrWhiteSpace(fallbackConnectionString))
        {
            return fallbackConnectionString;
        }

        var host = _configuration["Redis:Host"];
        if (string.IsNullOrWhiteSpace(host))
        {
            return null;
        }

        var port = _configuration.GetValue("Redis:Port", 6379);
        var password = _configuration["Redis:Password"];
        var database = _configuration.GetValue("Redis:Database", 0);
        var timeout = _configuration.GetValue("Redis:Timeout", 5000);

        var parts = new List<string>
        {
            $"{host}:{port}",
            $"defaultDatabase={database}",
            $"connectTimeout={timeout}",
            $"abortConnect=false"
        };

        if (!string.IsNullOrWhiteSpace(password))
        {
            parts.Add($"password={password}");
        }

        return string.Join(',', parts);
    }
}

