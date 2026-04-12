using System.Diagnostics;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using RoomManagerment.Shared.Messaging;

namespace Property.Infrastructure.Services;

public sealed class AppRequestSender(IServiceProvider serviceProvider, ILogger<AppRequestSender> logger) : IAppSender
{
    public async Task<TResponse> Send<TResponse>(IAppRequest<TResponse> request, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(request);
        var requestName = request.GetType().Name;
        logger.LogInformation("Handling {RequestName}", requestName);
        var sw = Stopwatch.StartNew();
        try
        {
            var handlerInterface = typeof(IAppRequestHandler<,>).MakeGenericType(request.GetType(), typeof(TResponse));
            var handler = serviceProvider.GetRequiredService(handlerInterface);
            var handleMethod = handler.GetType().GetMethod("Handle")
                ?? throw new InvalidOperationException($"Handler {handler.GetType().Name} has no Handle method.");
            var task = (Task)handleMethod.Invoke(handler, [request, cancellationToken])!;
            await task.ConfigureAwait(false);
            var resultProperty = task.GetType().GetProperty("Result")
                ?? throw new InvalidOperationException("Handler did not return a Task with Result.");
            var response = resultProperty.GetValue(task);
            sw.Stop();
            logger.LogInformation("Handled {RequestName} in {ElapsedMs}ms", requestName, sw.ElapsedMilliseconds);
            return response is null ? default! : (TResponse)response;
        }
        catch (Exception ex) when (ex is not OperationCanceledException)
        {
            logger.LogError(ex, "Unhandled exception in handler for {RequestName}", requestName);
            throw;
        }
    }
}
