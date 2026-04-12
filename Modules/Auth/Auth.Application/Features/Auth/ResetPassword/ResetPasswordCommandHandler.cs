using Auth.Application.Common;
using Auth.Application.Dtos;
using Auth.Application.Services;
using RoomManagerment.Shared.Messaging;

namespace Auth.Application.Features.Auth.ResetPassword;

public sealed class ResetPasswordCommandHandler(IAuthApplicationService authService)
    : IAppRequestHandler<ResetPasswordCommand, Result>
{
    public Task<Result> Handle(ResetPasswordCommand request, CancellationToken cancellationToken)
        => authService.ResetPasswordAsync(
            new ResetPasswordRequest(request.Email, request.OtpCode, request.NewPassword),
            cancellationToken);
}
