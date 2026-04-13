using Lease.Application;
using Lease.Application.Services;
using Lease.Infrastructure.Consumers;
using Lease.Domain.Repositories;
using Lease.Infrastructure.Repositories;
using Lease.Infrastructure.Services;
using FluentValidation;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using RoomManagerment.Messaging.Extensions;
using RoomManagerment.Lease.DatabaseSpecific;
using RoomManagerment.Shared.Messaging;

namespace Lease.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddValidatorsFromAssembly(typeof(MediatorAssemblyMarker).Assembly);

        services.AddRabbitMqMessaging(configuration, x =>
        {
            x.AddConsumer<LeaseExpiringCheckRequestedConsumer>();
            x.AddConsumer<LeaseExpirySweepRequestedConsumer>();
        });

        services.AddSingleton<LeaseRabbitMqIntegrationEventPublisher>();
        services.AddSingleton<ILeaseIntegrationEventPublisher>(sp =>
            sp.GetRequiredService<LeaseRabbitMqIntegrationEventPublisher>());
        services.AddHostedService<LeaseRabbitMqIntegrationEventBackgroundService>();

        services.AddScoped<IDataAccessAdapterFactory, DataAccessAdapterFactory>();
        services.AddLeaseApplicationRequestHandlers();
        services.AddScoped<IAppSender, AppRequestSender>();
        services.AddScoped<IMediatorGateway, MediatorGateway>();
        services.AddScoped<ILeaseApplicationService, LeaseApplicationService>();
        services.AddScoped<DataAccessAdapter>(provider =>
        {
            var factory = provider.GetRequiredService<IDataAccessAdapterFactory>();
            return (DataAccessAdapter)factory.CreateAdapter();
        });

        services.AddScoped<IUnitOfWork, UnitOfWork>();
        services.AddScoped<ILeaseRepository, LeaseRepository>();
        return services;
    }
}

