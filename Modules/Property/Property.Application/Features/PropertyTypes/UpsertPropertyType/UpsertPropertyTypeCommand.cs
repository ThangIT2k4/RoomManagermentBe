using Property.Application.Dtos;
using RoomManagerment.Shared.Messaging;

namespace Property.Application.Features.PropertyTypes.UpsertPropertyType;

public sealed record UpsertPropertyTypeCommand(Guid OrganizationId, Guid UserId, UpsertPropertyTypeRequest Request)
    : IAppRequest<PropertyTypeDto>;
