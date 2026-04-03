using Identity.Application.Common;
using MediatR;

namespace Identity.Application.Features.Auth.ChangePassword;

public sealed class ChangePasswordCommand : IRequest<Result<bool>>
{
    public required Guid UserId { get; init; }
    public required string CurrentPassword { get; init; }
    public required string NewPassword { get; init; }
}