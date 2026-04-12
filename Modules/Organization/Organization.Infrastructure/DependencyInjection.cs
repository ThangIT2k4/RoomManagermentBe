using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Organization.Application.Services;
using Organization.Domain.Repositories;
using Organization.Infrastructure.Repositories;
using Organization.Infrastructure.Services;
using RoomManagerment.Messaging.Extensions;
using RoomManagerment.Organizations.DatabaseSpecific;
using RoomManagerment.Shared.Messaging;

namespace Organization.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddRabbitMqMessaging(configuration);
        services.AddOrganizationApplicationRequestHandlers();
        services.AddScoped<IAppSender, AppRequestSender>();
        services.AddScoped<IMediatorGateway, MediatorGateway>();
        services.AddScoped<IDataAccessAdapterFactory, DataAccessAdapterFactory>();
        services.AddScoped<DataAccessAdapter>(provider =>
        {
            var factory = provider.GetRequiredService<IDataAccessAdapterFactory>();
            return (DataAccessAdapter)factory.CreateAdapter();
        });

        services.AddScoped<IUnitOfWork, UnitOfWork>();
        services.AddScoped<IOrganizationRepository, OrganizationRepository>();
        services.AddScoped<IOrganizationUserRepository, OrganizationUserRepository>();
        services.AddScoped<IOrganizationBankingRepository, OrganizationBankingRepository>();
        services.AddScoped<IOrganizationUserCapabilityRepository, OrganizationUserCapabilityRepository>();
        services.AddScoped<IOrganizationApplicationService, OrganizationApplicationService>();
        services.AddSingleton<IOrganizationCacheService, RedisOrganizationCacheService>();
        services.AddSingleton<RabbitMqIntegrationEventPublisher>();
        services.AddSingleton<IIntegrationEventPublisher>(sp => sp.GetRequiredService<RabbitMqIntegrationEventPublisher>());
        services.AddHostedService<RabbitMqIntegrationEventBackgroundService>();
        return services;
    }
}
