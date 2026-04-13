using Auth.Application.Common;
using Auth.Application.Dtos;
using Auth.Application.Services;
using RoomManagerment.Shared.Messaging;

namespace Auth.Application.Features.Auth.ResendVerifyEmailOtp;

public sealed class ResendVerifyEmailOtpCommandHandler(IAuthApplicationService authService)
    : IAppRequestHandler<ResendVerifyEmailOtpCommand, Result>
{
    public Task<Result> Handle(ResendVerifyEmailOtpCommand request, CancellationToken cancellationToken)
        => authService.ResendVerifyEmailOtpAsync(new ResendVerifyEmailOtpRequest(request.Email, request.Password), cancellationToken);
}
