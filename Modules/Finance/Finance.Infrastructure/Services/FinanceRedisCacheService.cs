using System.Text.Json;
using Finance.Application.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using StackExchange.Redis;

namespace Finance.Infrastructure.Services;

public sealed class FinanceRedisCacheService : ICacheService
{
    private readonly IConfiguration _configuration;
    private readonly ILogger<FinanceRedisCacheService> _logger;
    private readonly SemaphoreSlim _lock = new(1, 1);
    private IConnectionMultiplexer? _redis;
    private IDatabase? _database;

    public FinanceRedisCacheService(IConfiguration configuration, ILogger<FinanceRedisCacheService> logger)
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
            _logger.LogWarning(ex, "Redis get failed for key {CacheKey}.", key);
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
            _logger.LogWarning(ex, "Redis set failed for key {CacheKey}.", key);
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
            _logger.LogWarning(ex, "Redis remove failed for key {CacheKey}.", key);
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
                _logger.LogWarning(ex, "Redis connection failed. Finance cache will operate in no-op mode.");
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
        var explicitConnectionString = _configuration["Redis:ConnectionString"];
        if (!string.IsNullOrWhiteSpace(explicitConnectionString))
        {
            return explicitConnectionString;
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
            "abortConnect=false"
        };

        if (!string.IsNullOrWhiteSpace(password))
        {
            parts.Add($"password={password}");
        }

        return string.Join(',', parts);
    }
}
