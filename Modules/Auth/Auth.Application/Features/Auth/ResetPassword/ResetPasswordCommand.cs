using Auth.Application.Common;
using RoomManagerment.Shared.Messaging;

namespace Auth.Application.Features.Auth.ResetPassword;

public sealed record ResetPasswordCommand(string Email, string OtpCode, string NewPassword) : IAppRequest<Result>;
