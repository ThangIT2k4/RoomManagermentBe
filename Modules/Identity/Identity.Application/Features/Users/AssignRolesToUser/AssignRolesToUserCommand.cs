using Identity.Application.Common;
using MediatR;

namespace Identity.Application.Features.Users.AssignRolesToUser;

public sealed record AssignRolesToUserCommand(
    Guid UserId,
    IReadOnlyCollection<Guid> RoleIds) : IRequest<Result>;

