using Auth.Application.Common;
using Auth.Application.Dtos;
using Auth.Application.Services;
using RoomManagerment.Shared.Messaging;

namespace Auth.Application.Features.Auth.Sessions.LogoutAllSessions;

public sealed class LogoutAllSessionsCommandHandler(IAuthApplicationService authService)
    : IAppRequestHandler<LogoutAllSessionsCommand, Result>
{
    public Task<Result> Handle(LogoutAllSessionsCommand request, CancellationToken cancellationToken)
        => authService.LogoutAllSessionsAsync(new LogoutAllSessionsRequest(request.UserId), cancellationToken);
}
