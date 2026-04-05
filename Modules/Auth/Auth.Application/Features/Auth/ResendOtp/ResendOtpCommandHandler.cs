using Auth.Application.Common;
using Auth.Application.Dtos;
using Auth.Application.Services;
using MediatR;

namespace Auth.Application.Features.Auth.ResendOtp;

public sealed class ResendOtpCommandHandler(IAuthApplicationService authService)
    : IRequestHandler<ResendOtpCommand, Result>
{
    public Task<Result> Handle(ResendOtpCommand request, CancellationToken cancellationToken)
        => authService.ResendOtpAsync(new ResendOtpRequest(request.Email, request.Purpose, request.UserId), cancellationToken);
}
