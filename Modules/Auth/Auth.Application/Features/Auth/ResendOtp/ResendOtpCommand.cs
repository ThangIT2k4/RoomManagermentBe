using Auth.Application.Common;
using Auth.Application.Dtos;
using MediatR;

namespace Auth.Application.Features.Auth.ResendOtp;

public sealed record ResendOtpCommand(string Email, OtpPurpose Purpose, Guid? UserId = null) : IRequest<Result>;
