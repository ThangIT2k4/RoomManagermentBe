using Auth.Application.Common;
using Auth.Application.Dtos;
using Auth.Application.Services;
using MediatR;

namespace Auth.Application.Features.Auth.VerifyOtp;

public sealed class VerifyOtpCommandHandler(IAuthApplicationService authService)
    : IRequestHandler<VerifyOtpCommand, Result>
{
    public Task<Result> Handle(VerifyOtpCommand request, CancellationToken cancellationToken)
        => authService.VerifyOtpAsync(new VerifyOtpRequest(request.Email, request.Purpose, request.OtpCode), cancellationToken);
}
