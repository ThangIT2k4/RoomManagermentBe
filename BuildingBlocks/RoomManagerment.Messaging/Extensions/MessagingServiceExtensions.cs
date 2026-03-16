using MassTransit;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using RoomManagerment.Messaging.Configuration;

namespace RoomManagerment.Messaging.Extensions;

public static class MessagingServiceExtensions
{
    public static IServiceCollection AddRabbitMqMessaging(
        this IServiceCollection services,
        IConfiguration configuration,
        Action<IBusRegistrationConfigurator>? configureConsumers = null
    )
    {
        var options = configuration
                          .GetSection(RabbitMqOptions.Section)
                          .Get<RabbitMqOptions>()
                          ?? throw new InvalidOperationException(
                            $"Missing section '{RabbitMqOptions.Section}' in configuration."
                          );

        services
            .AddOptions<RabbitMqOptions>()
            .Bind(configuration.GetSection(RabbitMqOptions.Section))
            .ValidateDataAnnotations()
            .Validate(o => !string.IsNullOrWhiteSpace(o.Host), "RabbitMq:Host is required")
            .Validate(o => !string.IsNullOrWhiteSpace(o.Username), "RabbitMq:Username is required")
            .Validate(o => !string.IsNullOrWhiteSpace(o.Password), "RabbitMq:Password is required")
            .ValidateOnStart();


        services.AddMassTransit(x =>
        {
            configureConsumers?.Invoke(x);
            
            x.UsingRabbitMq((ctx, cfg) =>
            {
                cfg.Host(options.Host, (ushort)options.Port, options.VirtualHost, h =>
                {
                    h.Username(options.Username);
                    h.Password(options.Password);
                });
                
                cfg.UseMessageRetry(r => r.Incremental(
                    options.RetryCount,
                    TimeSpan.FromMilliseconds(options.RetryIntervalMs),
                    TimeSpan.FromMilliseconds(options.RetryIntervalMs)
                ));
                
                cfg.ConfigureEndpoints(ctx);
            });
        });
        
        return services;
    }
    
}
