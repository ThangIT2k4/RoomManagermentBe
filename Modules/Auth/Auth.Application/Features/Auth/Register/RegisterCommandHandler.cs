using Auth.Application.Common;
using Auth.Application.Dtos;
using Auth.Application.Services;
using MediatR;

namespace Auth.Application.Features.Auth.Register;

public sealed class RegisterCommandHandler(IAuthApplicationService authService)
    : IRequestHandler<RegisterCommand, Result<RegisterResult>>
{
    public Task<Result<RegisterResult>> Handle(RegisterCommand request, CancellationToken cancellationToken)
    {
        var dto = new RegisterRequest(request.Email, request.Password, request.FullName, request.Username, request.Phone);
        return authService.RegisterAsync(dto, cancellationToken);
    }
}
