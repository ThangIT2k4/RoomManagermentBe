namespace Property.Application.Services;

public interface IMediatorGateway
{
    Task<TResponse> SendAsync<TResponse>(object request, CancellationToken cancellationToken = default);
}
