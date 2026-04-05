using Auth.Application.Common;
using Auth.Application.Dtos;
using MediatR;

namespace Auth.Application.Features.Auth.VerifyOtp;

public sealed record VerifyOtpCommand(string Email, OtpPurpose Purpose, string OtpCode) : IRequest<Result>;
