using Auth.Application.Common;
using Auth.Application.Dtos;
using Auth.Application.Services;
using MediatR;

namespace Auth.Application.Features.Auth.Logout;

public sealed class LogoutCommandHandler(IAuthApplicationService authService)
    : IRequestHandler<LogoutCommand, Result>
{
    public Task<Result> Handle(LogoutCommand request, CancellationToken cancellationToken)
        => authService.LogoutAsync(new LogoutRequest(request.SessionId), cancellationToken);
}
