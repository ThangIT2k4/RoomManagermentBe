using Microsoft.Extensions.DependencyInjection;
using Property.Application;
using RoomManagerment.Shared.Messaging;

namespace Property.Infrastructure;

public static class PropertyRequestHandlerRegistration
{
    public static IServiceCollection AddPropertyApplicationRequestHandlers(this IServiceCollection services)
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
