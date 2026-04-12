using Auth.Application.Common;
using RoomManagerment.Shared.Messaging;

namespace Auth.Application.Features.Auth.VerifyEmail;

public sealed record VerifyEmailCommand(string Email, string OtpCode) : IAppRequest<Result>;
