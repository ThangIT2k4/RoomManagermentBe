using RoomManagerment.Shared.Messaging;

namespace Auth.Application.Features.Users.AssignRole;

public sealed record AssignRoleCommand(Guid OrganizationId, Guid UserId, Guid RoleId) : IAppRequest<Result>;
