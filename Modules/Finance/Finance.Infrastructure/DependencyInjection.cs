using Finance.Application;
using Finance.Application.Services;
using Finance.Domain.Repositories;
using Finance.Infrastructure.Consumers;
using Finance.Infrastructure.Repositories;
using Finance.Infrastructure.Services;
using FluentValidation;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using RoomManagerment.Finance.DatabaseSpecific;
using RoomManagerment.Messaging.Extensions;
using RoomManagerment.Shared.Messaging;

namespace Finance.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddFinanceInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddScoped<IDataAccessAdapterFactory, DataAccessAdapterFactory>();
        services.AddValidatorsFromAssembly(typeof(MediatorAssemblyMarker).Assembly);
        services.AddFinanceApplicationRequestHandlers();
        services.AddScoped<IAppSender, AppRequestSender>();

        services.AddSingleton<ICacheService, FinanceRedisCacheService>();

        var messagingEnabled = configuration.GetValue("Finance:Messaging:Enabled", false);
        if (messagingEnabled)
        {
            services.AddRabbitMqMessaging(
                configuration,
                x =>
                {
                    x.AddConsumer<FinanceAutoInvoiceConsumer>();
                    x.AddConsumer<FinanceOverdueSweepConsumer>();
                });
            services.AddSingleton<FinanceRabbitMqIntegrationEventPublisher>();
            services.AddSingleton<IFinanceIntegrationEventPublisher>(sp =>
                sp.GetRequiredService<FinanceRabbitMqIntegrationEventPublisher>());
            services.AddHostedService<FinanceRabbitMqIntegrationEventBackgroundService>();
            services.AddSingleton<IFinanceScheduledJobPublisher, MassTransitFinanceScheduledJobPublisher>();
        }
        else
        {
            services.AddSingleton<IFinanceIntegrationEventPublisher, NoOpFinanceIntegrationEventPublisher>();
            services.AddSingleton<IFinanceScheduledJobPublisher, NoOpFinanceScheduledJobPublisher>();
        }

        services.AddScoped<DataAccessAdapter>(provider =>
        {
            var factory = provider.GetRequiredService<IDataAccessAdapterFactory>();
            return (DataAccessAdapter)factory.CreateAdapter();
        });

        services.AddScoped<IUnitOfWork, UnitOfWork>();
        services.AddScoped<IInvoiceRepository, InvoiceRepository>();
        services.AddScoped<IInvoiceItemRepository, InvoiceItemRepository>();
        services.AddScoped<IPaymentRepository, PaymentRepository>();
        services.AddScoped<IDepositRefundRepository, DepositRefundRepository>();
        services.AddScoped<ILeaseReadGateway, StubLeaseReadGateway>();
        services.AddScoped<IInvoiceNumberGenerator, DatabaseInvoiceNumberGenerator>();
        services.AddScoped<IOnlinePaymentInitiator, NoOpOnlinePaymentInitiator>();
        services.AddScoped<IPaymentWebhookHandler, NoOpPaymentWebhookHandler>();
        services.AddScoped<IFinanceApplicationService, FinanceApplicationService>();

        return services;
    }
}
