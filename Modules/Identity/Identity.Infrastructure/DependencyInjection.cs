using Identity.Application.Services;
using Identity.Domain.Repositories;
using Identity.Infrastructure.Options;
using Identity.Infrastructure.Repositories;
using Identity.Infrastructure.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using RoomManagerment.Identity.DatabaseSpecific;
using StackExchange.Redis;

namespace Identity.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration,
        IHostEnvironment environment)
    {
        // register data access adapter factory and adapter
        services.AddScoped<IDataAccessAdapterFactory, DataAccessAdapterFactory>();
        services.AddScoped<DataAccessAdapter>(provider =>
        {
            var factory = provider.GetRequiredService<IDataAccessAdapterFactory>();
            return (DataAccessAdapter)factory.CreateAdapter();
        });

        // register redis cache service
        var redisConfig = configuration.GetSection("Redis");
        var redisConnectionString = $"{redisConfig["Host"]}:{redisConfig["Port"]},password={redisConfig["Password"]},defaultDatabase={redisConfig["Database"]},connectTimeout={redisConfig["Timeout"]}";
        services.AddSingleton<IConnectionMultiplexer>(sp => ConnectionMultiplexer.Connect(redisConnectionString));
        services.AddScoped<ICacheService, RedisCacheService>();

        // session store: Redis (cùng store với Gateway để share session)
        var sessionInstanceName = configuration["Session:RedisInstanceName"] ?? "RoomManagermentGateway:";
        services.AddStackExchangeRedisCache(options =>
        {
            options.Configuration = redisConnectionString;
            options.InstanceName = sessionInstanceName;
        });

        // register session configuration (cookie name trùng Gateway: RoomManager.SessionId)
        services.AddSessionConfiguration(configuration, environment);

        // register mapping services
       
        // register unit of work and repositories
        services.AddScoped<IUnitOfWork, UnitOfWork>();
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IRoleRepository, RoleRepository>();
        services.AddScoped<IPermissionRepository, PermissionRepository>();
        services.AddScoped<IMenuRepository, MenuRepository>();
        services.AddScoped<IMenuPermissionRepository, MenuPermissionRepository>();
        services.AddScoped<IUserProfileRepository, UserProfileRepository>();
        services.AddScoped<IUserRoleRepository, UserRoleRepository>();
        services.AddScoped<IRolePermissionRepository, RolePermissionRepository>();
        services.AddScoped<IUserPermissionRepository, UserPermissionRepository>();
        services.AddScoped<IUserMenuOverrideRepository, UserMenuOverrideRepository>();

        return services;
    }

    private static IServiceCollection AddSessionConfiguration(
        this IServiceCollection services,
        IConfiguration configuration,
        IHostEnvironment environment)
    {
        var sessionOptions = configuration.GetSection("Session").Get<SessionOptions>()
                             ?? new SessionOptions();

        services.AddSession(options =>
        {
            options.IdleTimeout = TimeSpan.FromMinutes(sessionOptions.IdleTimeout);
            options.Cookie.Name = sessionOptions.Cookie.Name ?? "RoomManager.SessionId";
            options.Cookie.HttpOnly = sessionOptions.Cookie.HttpOnly;
            options.Cookie.IsEssential = sessionOptions.Cookie.IsEssential;
            options.Cookie.SecurePolicy = environment.IsDevelopment()
                ? CookieSecurePolicy.SameAsRequest
                : sessionOptions.Cookie.SecurePolicy;
            options.Cookie.SameSite = sessionOptions.Cookie.SameSite;
        });

        return services;
    }
}