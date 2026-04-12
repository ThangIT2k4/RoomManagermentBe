using Organization.Application.Common;
using RoomManagerment.Shared.Messaging;

namespace Organization.Application.Features.Members.CanMember;

public sealed record CanMemberQuery(Guid OrganizationId, Guid UserId, string CapabilityKey)
    : IAppRequest<Result<bool>>;
