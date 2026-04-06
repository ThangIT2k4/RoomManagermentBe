using CRM.Application.Services;
using CRM.Domain.Repositories;
using CRM.Infrastructure.Repositories;
using CRM.Infrastructure.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using RoomManagerment.CRM.DatabaseSpecific;
using RoomManagerment.Messaging.Configuration;
using RoomManagerment.Messaging.Extensions;

namespace CRM.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddScoped<IDataAccessAdapterFactory, DataAccessAdapterFactory>();
        services.AddRabbitMqMessaging(configuration);
        services.AddScoped<ICrmApplicationService, CrmApplicationService>();
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
        services.AddScoped<ILeadRepository, LeadRepository>();
        return services;
    }
}

