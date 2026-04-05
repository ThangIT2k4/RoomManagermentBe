using Auth.Application.Common;
using Auth.Application.Dtos;
using Auth.Application.Services;
using MediatR;

namespace Auth.Application.Features.Auth.ForgotPassword;

public sealed class ForgotPasswordCommandHandler(IAuthApplicationService authService)
    : IRequestHandler<ForgotPasswordCommand, Result>
{
    public Task<Result> Handle(ForgotPasswordCommand request, CancellationToken cancellationToken)
        => authService.ForgotPasswordAsync(new ForgotPasswordRequest(request.Email), cancellationToken);
}
