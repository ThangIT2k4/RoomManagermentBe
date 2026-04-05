using Auth.Application.Common;
using MediatR;

namespace Auth.Application.Features.Auth.VerifyEmail;

public sealed record VerifyEmailCommand(string Email, string OtpCode) : IRequest<Result>;
