using Auth.Application.Common;
using Auth.Application.Dtos;
using MediatR;

namespace Auth.Application.Features.Auth.SendOtp;

public sealed record SendOtpCommand(string Email, OtpPurpose Purpose, Guid? UserId = null) : IRequest<Result>;
