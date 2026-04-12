using MediatR;
using Microsoft.Extensions.Logging;

namespace RoomManagerment.Shared.MediatR;

/// <summary>
/// Ensures handler exceptions are logged with MediatR request context before propagating to global middleware.
/// </summary>
public sealed class ExceptionBehavior<TRequest, TResponse>(ILogger<ExceptionBehavior<TRequest, TResponse>> logger)
    : IPipelineBehavior<TRequest, TResponse>
    where TRequest : notnull
{
    public async Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        try
        {
            return await next();
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Unhandled exception in MediatR handler for {RequestType}", typeof(TRequest).Name);
            throw;
        }
    }
}
