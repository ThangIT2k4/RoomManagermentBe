using Auth.Application.Common;
using MediatR;

namespace Auth.Application.Features.Auth.ForgotPassword;

public sealed record ForgotPasswordCommand(string Email) : IRequest<Result>;
