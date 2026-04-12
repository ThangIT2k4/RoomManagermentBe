using Property.Application.Dtos;
using RoomManagerment.Shared.Messaging;

namespace Property.Application.Features.PropertyTypes.GetPropertyTypes;

public sealed record GetPropertyTypesQuery(Guid OrganizationId) : IAppRequest<IReadOnlyList<PropertyTypeDto>>;
