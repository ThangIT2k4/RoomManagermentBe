using Property.Application.Dtos;
using RoomManagerment.Shared.Messaging;

namespace Property.Application.Features.Units.UpdateUnit;

public sealed record UpdateUnitCommand(Guid OrganizationId, Guid UserId, UpdateUnitRequest Request)
    : IAppRequest<UnitDto?>;
