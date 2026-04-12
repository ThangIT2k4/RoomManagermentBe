using Organization.Application.Common;
using RoomManagerment.Shared.Messaging;

namespace Organization.Application.Features.Members.ChangeMemberActivation;

public sealed record ChangeMemberActivationCommand(Guid OrganizationId, Guid UserId, bool IsActive, Guid ActorUserId, string? Reason)
    : IAppRequest<Result>;
