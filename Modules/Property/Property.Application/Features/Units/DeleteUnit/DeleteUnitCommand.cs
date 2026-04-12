using RoomManagerment.Shared.Messaging;

namespace Property.Application.Features.Units.DeleteUnit;

public sealed record DeleteUnitCommand(Guid OrganizationId, Guid UserId, Guid UnitId)
    : IAppRequest<bool>;
