using Auth.Application.Common;
using Auth.Application.Dtos;
using Auth.Application.Services;
using RoomManagerment.Shared.Messaging;

namespace Auth.Application.Features.Auth.ForgotPassword;

public sealed class ForgotPasswordCommandHandler(IAuthApplicationService authService)
    : IAppRequestHandler<ForgotPasswordCommand, Result>
{
    public Task<Result> Handle(ForgotPasswordCommand request, CancellationToken cancellationToken)
        => authService.ForgotPasswordAsync(new ForgotPasswordRequest(request.Email), cancellationToken);
}
