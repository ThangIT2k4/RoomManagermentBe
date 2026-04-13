using System.Text.Json;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Organization.Application.Dtos;
using Organization.Application.Services;
using StackExchange.Redis;

namespace Organization.Infrastructure.Services;

public sealed class RedisOrganizationCacheService : IOrganizationCacheService
{
    private readonly IConfiguration _configuration;
    private readonly ILogger<RedisOrganizationCacheService> _logger;
    private readonly SemaphoreSlim _lock = new(1, 1);
    private IConnectionMultiplexer? _redis;
    private IDatabase? _database;

    public RedisOrganizationCacheService(IConfiguration configuration, ILogger<RedisOrganizationCacheService> logger)
    {
        _configuration = configuration;
        _logger = logger;
    }

    public async Task<OrganizationDto?> GetOrganizationAsync(Guid organizationId, CancellationToken cancellationToken = default)
    {
        try
        {
            var database = await GetDatabaseAsync(cancellationToken);
            if (database is null)
            {
                return null;
            }

            var value = await database.StringGetAsync(Key(organizationId));
            return value.IsNullOrEmpty ? null : JsonSerializer.Deserialize<OrganizationDto>(value!);
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Redis lấy cache tổ chức thất bại với {OrganizationId}.", organizationId);
            return null;
        }
    }

    public async Task SetOrganizationAsync(OrganizationDto organization, TimeSpan ttl, CancellationToken cancellationToken = default)
    {
        try
        {
            var database = await GetDatabaseAsync(cancellationToken);
            if (database is null)
            {
                return;
            }

            await database.StringSetAsync(Key(organization.Id), JsonSerializer.Serialize(organization), ttl);
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Redis ghi cache tổ chức thất bại với {OrganizationId}.", organization.Id);
        }
    }

    public async Task RemoveOrganizationAsync(Guid organizationId, CancellationToken cancellationToken = default)
    {
        try
        {
            var database = await GetDatabaseAsync(cancellationToken);
            if (database is null)
            {
                return;
            }

            await database.KeyDeleteAsync(Key(organizationId));
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Redis xóa cache tổ chức thất bại với {OrganizationId}.", organizationId);
        }
    }

    private static string Key(Guid organizationId) => $"org:settings:{organizationId}";

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

            var connectionString =
                _configuration["Redis:Configuration"]
                ?? _configuration["Redis:ConnectionString"]
                ?? _configuration.GetConnectionString("Redis");
            if (string.IsNullOrWhiteSpace(connectionString))
            {
                return null;
            }

            try
            {
                _redis = await ConnectionMultiplexer.ConnectAsync(connectionString);
                _database = _redis.GetDatabase();
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Redis không khả dụng, cache chuyển sang chế độ no-op.");
                _redis = null;
                _database = null;
            }

            return _database;
        }
        finally
        {
            _lock.Release();
        }
    }
}
