namespace RoomManagerment.Shared.Messaging;

/// <summary>Marker: request (command/query) trả về <typeparamref name="TResponse"/>.</summary>
public interface IAppRequest<out TResponse> { }

/// <summary>Application handler — không phụ thuộc MediatR; đăng ký &amp; pipeline ở Infrastructure.</summary>
public interface IAppRequestHandler<in TRequest, TResponse>
    where TRequest : IAppRequest<TResponse>
{
    Task<TResponse> Handle(TRequest request, CancellationToken cancellationToken);
}

/// <summary>Điểm vào gửi request từ API; implementation nằm trong Infrastructure.</summary>
public interface IAppSender
{
    Task<TResponse> Send<TResponse>(IAppRequest<TResponse> request, CancellationToken cancellationToken);
}
