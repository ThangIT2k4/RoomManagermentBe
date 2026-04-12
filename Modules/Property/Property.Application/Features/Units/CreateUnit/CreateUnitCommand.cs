using Property.Application.Dtos;
using RoomManagerment.Shared.Messaging;

namespace Property.Application.Features.Units.CreateUnit;

public sealed record CreateUnitCommand(Guid OrganizationId, Guid UserId, CreateUnitRequest Request)
    : IAppRequest<UnitDto>;
