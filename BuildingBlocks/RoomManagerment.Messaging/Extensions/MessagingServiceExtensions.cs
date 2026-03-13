using MassTransit;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using RoomManagerment.Messaging.Configuration;

namespace RoomManagerment.Messaging.Extensions;

public static class MessagingServiceExtensions
{
    /// <summary>
    /// Đăng ký MassTransit + RabbitMQ với các consumer được chỉ định.
    /// Gọi từ Infrastructure/API Program.cs.
    /// </summary>
    /// <param name="services">DI container</param>
    /// <param name="configuration">App configuration</param>
    /// <param name="configureConsumers">Action để đăng ký consumer và queue</param>
    public static IServiceCollection AddRabbitMqMessaging(
        this IServiceCollection services,
        IConfiguration configuration,
        Action<IBusRegistrationConfigurator>? configureConsumers = null)
    {
        var options = configuration.GetSection(RabbitMqOptions.Section).Get<RabbitMqOptions>()
                      ?? new RabbitMqOptions();

        services.AddMassTransit(x =>
        {
            // Đăng ký consumer nếu có
            configureConsumers?.Invoke(x);

            x.UsingRabbitMq((ctx, cfg) =>
            {
                cfg.Host(options.Host, (ushort)options.Port, options.VirtualHost, h =>
                {
                    h.Username(options.Username);
                    h.Password(options.Password);
                });

                cfg.UseMessageRetry(r =>
                    r.Incremental(options.RetryCount,
                        TimeSpan.FromMilliseconds(options.RetryIntervalMs),
                        TimeSpan.FromMilliseconds(options.RetryIntervalMs)));

                // Tự động cấu hình endpoint cho các consumer đã đăng ký
                cfg.ConfigureEndpoints(ctx);
            });
        });

        return services;
    }
}
