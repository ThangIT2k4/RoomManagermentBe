using Identity.Application.Common;
using MediatR;

namespace Identity.Application.Features.Roles.GetRoleById;

public sealed record GetRoleByIdQuery(Guid RoleId) : IRequest<Result<RoleDto>>;

