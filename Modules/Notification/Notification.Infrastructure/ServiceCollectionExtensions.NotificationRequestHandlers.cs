using Microsoft.Extensions.DependencyInjection;
using Notification.Application;
using RoomManagerment.Shared.Messaging;

namespace Notification.Infrastructure;

public static class NotificationRequestHandlerRegistration
{
    public static IServiceCollection AddNotificationApplicationRequestHandlers(this IServiceCollection services)
    {
        var assembly = typeof(MediatorAssemblyMarker).Assembly;
        foreach (var type in assembly.GetTypes().Where(t => t is { IsClass: true, IsAbstract: false }))
        {
            foreach (var iface in type.GetInterfaces())
            {
                if (iface is { IsGenericType: true } && iface.GetGenericTypeDefinition() == typeof(IAppRequestHandler<,>))
                    services.AddScoped(iface, type);
            }
        }

        return services;
    }
}
