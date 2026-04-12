using Auth.Application.Common;
using RoomManagerment.Shared.Messaging;

namespace Auth.Application.Features.Auth.ForgotPassword;

public sealed record ForgotPasswordCommand(string Email) : IAppRequest<Result>;
