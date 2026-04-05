using Auth.Application.Common;
using MediatR;

namespace Auth.Application.Features.Auth.ChangePassword;

public sealed record ChangePasswordCommand(
    Guid UserId,
    string CurrentPassword,
    string NewPassword,
    string? RetainSessionId = null) : IRequest<Result>;
