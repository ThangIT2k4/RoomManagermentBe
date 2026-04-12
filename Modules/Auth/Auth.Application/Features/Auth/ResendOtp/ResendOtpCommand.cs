using Auth.Application.Common;
using Auth.Application.Dtos;
using RoomManagerment.Shared.Messaging;

namespace Auth.Application.Features.Auth.ResendOtp;

public sealed record ResendOtpCommand(string Email, OtpPurpose Purpose, Guid? UserId = null) : IAppRequest<Result>;
