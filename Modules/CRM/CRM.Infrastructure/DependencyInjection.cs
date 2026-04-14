using CRM.Application;
using CRM.Application.Services;
using CRM.Domain.Repositories;
using CRM.Infrastructure.Repositories;
using CRM.Infrastructure.Services;
using FluentValidation;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using RoomManagerment.CRM.DatabaseSpecific;
using RoomManagerment.Messaging.Configuration;
using RoomManagerment.Messaging.Extensions;
using RoomManagerment.Shared.Messaging;

namespace CRM.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddScoped<IDataAccessAdapterFactory, DataAccessAdapterFactory>();
        services.AddValidatorsFromAssembly(typeof(MediatorAssemblyMarker).Assembly);
        services.AddCrmApplicationRequestHandlers();
        services.AddScoped<IAppSender, AppRequestSender>();

        services.AddStackExchangeRedisCache(options =>
        {
            options.Configuration = RedisServiceExtensions.ResolveConnectionString(configuration);
            options.InstanceName = "crm";
        });
        services.AddRabbitMqMessaging(configuration);
        services.AddScoped<ICrmApplicationService, CrmApplicationService>();
        services.AddScoped<ICrmCacheService, RedisCrmCacheService>();
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
        services.AddScoped<IViewingRepository, ViewingRepository>();
        services.AddScoped<IBookingDepositRepository, BookingDepositRepository>();
        services.AddScoped<IReviewRepository, ReviewRepository>();
        services.AddScoped<IReviewReplyRepository, ReviewReplyRepository>();
        services.AddScoped<ICommissionPolicyRepository, CommissionPolicyRepository>();
        services.AddScoped<ICommissionEventRepository, CommissionEventRepository>();
        return services;
    }
}
