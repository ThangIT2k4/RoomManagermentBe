using Auth.Application.Common;
using Auth.Application.Dtos;
using MediatR;

namespace Auth.Application.Features.Auth.Roles.GetUserRoles;

public sealed record GetUserRolesQuery(Guid UserId, Guid? OrganizationId = null) : IRequest<Result<IReadOnlyList<RoleDto>>>;
