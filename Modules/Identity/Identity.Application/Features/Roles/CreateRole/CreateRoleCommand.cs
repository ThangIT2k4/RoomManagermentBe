using Identity.Application.Common;
using MediatR;

namespace Identity.Application.Features.Roles.CreateRole;

public sealed record CreateRoleCommand(
    string Code,
    string Name) : IRequest<Result<CreateRoleResult>>;

