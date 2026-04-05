using Auth.Application.Common;
using Auth.Application.Dtos;
using Auth.Application.Services;
using MediatR;

namespace Auth.Application.Features.Auth.Login;

public sealed class LoginCommandHandler(IAuthApplicationService authService)
    : IRequestHandler<LoginCommand, Result<LoginResult>>
{
    public Task<Result<LoginResult>> Handle(LoginCommand request, CancellationToken cancellationToken)
    {
        var dto = new LoginRequest(
            request.Login,
            request.Password,
            request.IpAddress,
            request.UserAgent,
            request.RememberMe);
        return authService.LoginAsync(dto, cancellationToken);
    }
}
