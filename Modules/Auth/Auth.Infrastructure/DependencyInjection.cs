using Auth.Application;
using Auth.Application.Services;
using Auth.Domain.Repositories;
using Auth.Infrastructure.Repositories;
using Auth.Infrastructure.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MediatR;
using RoomManagerment.Auth.DatabaseSpecific;
using RoomManagerment.Messaging.Configuration;
using RoomManagerment.Messaging.Extensions;

namespace Auth.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddScoped<IDataAccessAdapterFactory, DataAccessAdapterFactory>();
        services.AddApplication();
        services.AddRabbitMqMessaging(configuration);
        services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(MediatorAssemblyMarker).Assembly));
        services.AddScoped<IMediatorGateway, MediatorGateway>();
        services.AddScoped<IPasswordHasher, BcryptPasswordHasher>();
        services.AddScoped<IOrganizationMembershipGateway, NoOpOrganizationMembershipGateway>();
        services.AddScoped<IAuthApplicationService, AuthApplicationService>();
        services.AddSingleton<ICacheService, RedisCacheService>();
        services.AddSingleton<RabbitMqIntegrationEventPublisher>();
        services.AddSingleton<IIntegrationEventPublisher>(sp => sp.GetRequiredService<RabbitMqIntegrationEventPublisher>());
        services.AddHostedService<RabbitMqIntegrationEventBackgroundService>();
        services.AddOptions<RabbitMqOptions>().Bind(configuration.GetSection(RabbitMqOptions.Section)).ValidateOnStart();
        services.AddScoped<DataAccessAdapter>(provider =>
        {
            var factory = provider.GetRequiredService<IDataAccessAdapterFactory>();
            return (DataAccessAdapter)factory.CreateAdapter();
        });

        services.AddScoped<IUnitOfWork, UnitOfWork>();
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<ISessionRepository, SessionRepository>();
        services.AddScoped<IUserProfileRepository, UserProfileRepository>();
        services.AddScoped<IRoleRepository, RoleRepository>();
        services.AddScoped<ICapabilityRepository, CapabilityRepository>();
        services.AddScoped<IEmailOtpRepository, EmailOtpRepository>();
        services.AddScoped<IAuditLogRepository, AuditLogRepository>();
        return services;
    }
}

