using Auth.Application.Dtos;
using Auth.Application.Services;
using RoomManagerment.Shared.Messaging;

namespace Auth.Application.Features.Register;

public sealed class RegisterHandler(IAuthApplicationService authService)
    : IAppRequestHandler<RegisterCommand, Result<RegisterResult>>
{
    public Task<Result<RegisterResult>> Handle(RegisterCommand request, CancellationToken cancellationToken)
    {
        var dto = new RegisterRequest(request.Email, request.Password, request.FullName, request.Username, request.Phone);
        return authService.RegisterAsync(dto, cancellationToken);
    }
}
