using Auth.Application.Common;
using Auth.Application.Dtos;
using Auth.Application.Services;
using MediatR;

namespace Auth.Application.Features.Auth.ChangePassword;

public sealed class ChangePasswordCommandHandler(IAuthApplicationService authService)
    : IRequestHandler<ChangePasswordCommand, Result>
{
    public Task<Result> Handle(ChangePasswordCommand request, CancellationToken cancellationToken)
        => authService.ChangePasswordAsync(
            new ChangePasswordRequest(request.UserId, request.CurrentPassword, request.NewPassword, request.RetainSessionId),
            cancellationToken);
}
