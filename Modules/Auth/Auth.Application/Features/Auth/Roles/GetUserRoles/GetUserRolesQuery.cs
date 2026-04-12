using Auth.Application.Common;
using Auth.Application.Dtos;
using RoomManagerment.Shared.Messaging;

namespace Auth.Application.Features.Auth.Roles.GetUserRoles;

public sealed record GetUserRolesQuery(Guid UserId, Guid? OrganizationId = null) : IAppRequest<Result<IReadOnlyList<RoleDto>>>;
