using Auth.Application.Common;
using Auth.Application.Dtos;
using Auth.Application.Services;
using RoomManagerment.Shared.Messaging;

namespace Auth.Application.Features.Auth.Sessions.CreateSession;

public sealed class CreateSessionCommandHandler(IAuthApplicationService authService)
    : IAppRequestHandler<CreateSessionCommand, Result<SessionDto>>
{
    public Task<Result<SessionDto>> Handle(CreateSessionCommand request, CancellationToken cancellationToken)
        => authService.CreateSessionAsync(new CreateSessionRequest(request.UserId, request.IpAddress, request.UserAgent, request.RememberMe), cancellationToken);
}
