using Auth.Application.Common;
using Auth.Application.Dtos;
using Auth.Application.Services;
using MediatR;

namespace Auth.Application.Features.Auth.VerifyEmail;

public sealed class VerifyEmailCommandHandler(IAuthApplicationService authService)
    : IRequestHandler<VerifyEmailCommand, Result>
{
    public Task<Result> Handle(VerifyEmailCommand request, CancellationToken cancellationToken)
        => authService.VerifyEmailAsync(new VerifyEmailRequest(request.Email, request.OtpCode), cancellationToken);
}
