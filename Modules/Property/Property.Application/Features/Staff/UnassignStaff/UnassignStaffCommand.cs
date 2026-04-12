using RoomManagerment.Shared.Messaging;

namespace Property.Application.Features.Staff.UnassignStaff;

public sealed record UnassignStaffCommand(Guid OrganizationId, Guid ActorUserId, Guid PropertyId, Guid TargetUserId)
    : IAppRequest<bool>;
