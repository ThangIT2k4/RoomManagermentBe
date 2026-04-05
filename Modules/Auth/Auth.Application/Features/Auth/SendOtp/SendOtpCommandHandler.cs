using Auth.Application.Common;
using Auth.Application.Dtos;
using Auth.Application.Services;
using MediatR;

namespace Auth.Application.Features.Auth.SendOtp;

public sealed class SendOtpCommandHandler(IAuthApplicationService authService)
    : IRequestHandler<SendOtpCommand, Result>
{
    public Task<Result> Handle(SendOtpCommand request, CancellationToken cancellationToken)
        => authService.SendOtpAsync(new SendOtpRequest(request.Email, request.Purpose, request.UserId), cancellationToken);
}
