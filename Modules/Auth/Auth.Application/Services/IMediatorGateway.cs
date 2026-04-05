namespace Auth.Application.Services;

public interface IMediatorGateway
{
    Task<TResponse> SendAsync<TResponse>(object request, CancellationToken cancellationToken = default);

    Task PublishAsync(object notification, CancellationToken cancellationToken = default);
}

