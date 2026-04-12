using Auth.Application.Common;
using Auth.Application.Dtos;
using RoomManagerment.Shared.Messaging;

namespace Auth.Application.Features.Auth.SendOtp;

public sealed record SendOtpCommand(string Email, OtpPurpose Purpose, Guid? UserId = null) : IAppRequest<Result>;
