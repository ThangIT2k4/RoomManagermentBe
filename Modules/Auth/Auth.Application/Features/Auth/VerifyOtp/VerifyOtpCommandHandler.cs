using Auth.Application.Common;
using Auth.Application.Dtos;
using Auth.Application.Services;
using RoomManagerment.Shared.Messaging;

namespace Auth.Application.Features.Auth.VerifyOtp;

public sealed class VerifyOtpCommandHandler(IAuthApplicationService authService)
    : IAppRequestHandler<VerifyOtpCommand, Result>
{
    public Task<Result> Handle(VerifyOtpCommand request, CancellationToken cancellationToken)
        => authService.VerifyOtpAsync(new VerifyOtpRequest(request.Email, request.Purpose, request.OtpCode), cancellationToken);
}
