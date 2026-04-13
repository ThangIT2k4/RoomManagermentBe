using Auth.Application.Common;
using RoomManagerment.Shared.Messaging;

namespace Auth.Application.Features.Auth.ResendVerifyEmailOtp;

public sealed record ResendVerifyEmailOtpCommand(string Email, string Password) : IAppRequest<Result>;
