using Organization.Application.Services;
using RoomManagerment.Shared.Messaging;

namespace Organization.Infrastructure.Services;

public sealed class MediatorGateway(IAppSender sender) : IMediatorGateway
{
    public Task<TResponse> SendAsync<TResponse>(object request, CancellationToken cancellationToken = default)
    {
        if (request is not IAppRequest<TResponse> appRequest)
            throw new ArgumentException(
                $"Kiểu request {request.GetType().Name} phải triển khai IAppRequest<{typeof(TResponse).Name}>.",
                nameof(request));

        return sender.Send(appRequest, cancellationToken);
    }

    public Task PublishAsync(object notification, CancellationToken cancellationToken = default)
        => Task.CompletedTask;
}
