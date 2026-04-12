using Property.Application.Dtos;
using RoomManagerment.Shared.Messaging;

namespace Property.Application.Features.Properties.UpdateProperty;

public sealed record UpdatePropertyCommand(Guid OrganizationId, Guid UserId, UpdatePropertyRequest Request)
    : IAppRequest<PropertyDto?>;
