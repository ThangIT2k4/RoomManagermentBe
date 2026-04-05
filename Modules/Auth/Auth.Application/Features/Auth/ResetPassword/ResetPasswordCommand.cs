using Auth.Application.Common;
using MediatR;

namespace Auth.Application.Features.Auth.ResetPassword;

public sealed record ResetPasswordCommand(string Email, string OtpCode, string NewPassword) : IRequest<Result>;
