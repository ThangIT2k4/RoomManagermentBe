using Auth.Application.Common;
using Auth.Application.Dtos;
using Auth.Application.Services;
using MediatR;

namespace Auth.Application.Features.Auth.ResetPassword;

public sealed class ResetPasswordCommandHandler(IAuthApplicationService authService)
    : IRequestHandler<ResetPasswordCommand, Result>
{
    public Task<Result> Handle(ResetPasswordCommand request, CancellationToken cancellationToken)
        => authService.ResetPasswordAsync(
            new ResetPasswordRequest(request.Email, request.OtpCode, request.NewPassword),
            cancellationToken);
}
