using RoomManagerment.Shared.Messaging;

namespace Property.Application.Features.PropertyTypes.DeletePropertyType;

public sealed record DeletePropertyTypeCommand(Guid OrganizationId, Guid UserId, Guid PropertyTypeId)
    : IAppRequest<bool>;
