using MediatR;
using Organization.Application.Services;

namespace Organization.Infrastructure.Services;

public sealed class MediatorGateway(IMediator mediator) : IMediatorGateway
{
    public Task<TResponse> SendAsync<TResponse>(object request, CancellationToken cancellationToken = default)
        => mediator.Send((dynamic)request, cancellationToken);

    public Task PublishAsync(object notification, CancellationToken cancellationToken = default)
        => mediator.Publish(notification, cancellationToken);
}
