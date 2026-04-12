using Auth.Application.Common;
using Auth.Application.Dtos;
using Auth.Application.Services;
using RoomManagerment.Shared.Messaging;

namespace Auth.Application.Features.Auth.ResendOtp;

public sealed class ResendOtpCommandHandler(IAuthApplicationService authService)
    : IAppRequestHandler<ResendOtpCommand, Result>
{
    public Task<Result> Handle(ResendOtpCommand request, CancellationToken cancellationToken)
        => authService.ResendOtpAsync(new ResendOtpRequest(request.Email, request.Purpose, request.UserId), cancellationToken);
}
