using System.Diagnostics;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using RoomManagerment.Shared.Messaging;

namespace Notification.Infrastructure.Services;

public sealed class AppRequestSender(IServiceProvider serviceProvider, ILogger<AppRequestSender> logger) : IAppSender
{
    public async Task<TResponse> Send<TResponse>(IAppRequest<TResponse> request, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(request);
        var requestName = request.GetType().Name;
        logger.LogInformation("Đang xử lý {RequestName}", requestName);
        var sw = Stopwatch.StartNew();
        try
        {
            var handlerInterface = typeof(IAppRequestHandler<,>).MakeGenericType(request.GetType(), typeof(TResponse));
            var handler = serviceProvider.GetRequiredService(handlerInterface);
            var handleMethod = handler.GetType().GetMethod("Handle")
                ?? throw new InvalidOperationException($"Trình xử lý {handler.GetType().Name} không có phương thức Handle.");
            var task = (Task)handleMethod.Invoke(handler, [request, cancellationToken])!;
            await task.ConfigureAwait(false);
            var resultProperty = task.GetType().GetProperty("Result")
                ?? throw new InvalidOperationException("Trình xử lý không trả về Task có thuộc tính Result.");
            var response = resultProperty.GetValue(task);
            sw.Stop();
            logger.LogInformation("Đã xử lý {RequestName} trong {ElapsedMs}ms", requestName, sw.ElapsedMilliseconds);
            return response is null ? default! : (TResponse)response;
        }
        catch (Exception ex) when (ex is not OperationCanceledException)
        {
            logger.LogError(ex, "Ngoại lệ chưa được xử lý trong trình xử lý của {RequestName}", requestName);
            throw;
        }
    }
}
