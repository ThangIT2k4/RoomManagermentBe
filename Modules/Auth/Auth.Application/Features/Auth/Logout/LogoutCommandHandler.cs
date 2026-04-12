using Auth.Application.Common;
using Auth.Application.Dtos;
using Auth.Application.Services;
using RoomManagerment.Shared.Messaging;

namespace Auth.Application.Features.Auth.Logout;

public sealed class LogoutCommandHandler(IAuthApplicationService authService)
    : IAppRequestHandler<LogoutCommand, Result>
{
    public Task<Result> Handle(LogoutCommand request, CancellationToken cancellationToken)
        => authService.LogoutAsync(new LogoutRequest(request.SessionId), cancellationToken);
}
