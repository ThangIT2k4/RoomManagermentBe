using MediatR;
using Microsoft.Extensions.DependencyInjection;

namespace RoomManagerment.Shared.MediatR;

public static class DependencyInjection
{
    /// <summary>
    /// Registers cross-cutting MediatR pipeline behaviors (order: exception logging, validation, timing).
    /// </summary>
    public static MediatRServiceConfiguration AddRoomManagermentPipelineBehaviors(
        this MediatRServiceConfiguration configuration)
    {
        configuration.AddBehavior(typeof(IPipelineBehavior<,>), typeof(ExceptionBehavior<,>));
        configuration.AddBehavior(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));
        configuration.AddBehavior(typeof(IPipelineBehavior<,>), typeof(LoggingBehavior<,>));
        return configuration;
    }
}
