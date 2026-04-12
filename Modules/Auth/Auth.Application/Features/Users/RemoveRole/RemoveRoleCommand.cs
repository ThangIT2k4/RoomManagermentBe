using RoomManagerment.Shared.Messaging;

namespace Auth.Application.Features.Users.RemoveRole;

public sealed record RemoveRoleCommand(Guid OrganizationId, Guid UserId) : IAppRequest<Result>;
