using System.Diagnostics;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using RoomManagerment.Shared.Http;

namespace RoomManagerment.Shared.Middleware;

public sealed class GlobalExceptionMiddleware(RequestDelegate next, ILogger<GlobalExceptionMiddleware> logger)
{
    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await next(context);
        }
        catch (Exception ex)
        {
            var correlationId = context.Items[CorrelationIdConstants.HttpContextItemKey]?.ToString()
                ?? Activity.Current?.TraceId.ToString()
                ?? "unknown";

            logger.LogError(ex,
                "Unhandled exception. CorrelationId={CorrelationId} Path={Path}",
                correlationId,
                context.Request.Path);

            if (context.Response.HasStarted)
            {
                throw;
            }

            context.Response.Clear();
            context.Response.StatusCode = StatusCodes.Status500InternalServerError;
            context.Response.ContentType = "application/problem+json";

            var problem = new ProblemDetails
            {
                Type = "https://tools.ietf.org/html/rfc7231#section-6.6.1",
                Title = "Internal Server Error",
                Status = StatusCodes.Status500InternalServerError,
                Detail = "An unexpected error occurred.",
                Instance = context.Request.Path.Value
            };
            problem.Extensions["traceId"] = correlationId;

            await context.Response.WriteAsJsonAsync(problem);
        }
    }
}
