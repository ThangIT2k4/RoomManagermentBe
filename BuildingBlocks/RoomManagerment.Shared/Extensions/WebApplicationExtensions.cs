using Microsoft.AspNetCore.Builder;
using RoomManagerment.Shared.Middleware;

namespace RoomManagerment.Shared.Extensions;

public static class WebApplicationExtensions
{
    /// <summary>
    /// Registers correlation ID (LogContext) then global exception handling as JSON ProblemDetails.
    /// Place early in the pipeline (after optional exception details in dev if needed).
    /// </summary>
    public static WebApplication UseRoomManagermentExceptionHandling(this WebApplication app)
    {
        app.UseMiddleware<CorrelationIdMiddleware>();
        app.UseMiddleware<GlobalExceptionMiddleware>();
        return app;
    }
}
