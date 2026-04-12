using RoomManagerment.Shared.Messaging;

namespace Property.Application.Features.Units.SetUnitStatus;

public sealed record SetUnitStatusCommand(Guid OrganizationId, Guid UserId, Guid UnitId, short Status, string? Reason)
    : IAppRequest<bool>;
