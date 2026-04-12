using Lease.Application.Services;
using RoomManagerment.Shared.Messaging;

namespace Lease.Infrastructure.Services;

public sealed class MediatorGateway(IAppSender sender) : IMediatorGateway
{
    public Task<TResponse> SendAsync<TResponse>(object request, CancellationToken cancellationToken = default)
    {
        if (request is not IAppRequest<TResponse> appRequest)
            throw new ArgumentException(
                $"Request type {request.GetType().Name} must implement IAppRequest<{typeof(TResponse).Name}>.",
                nameof(request));

        return sender.Send(appRequest, cancellationToken);
    }
}
