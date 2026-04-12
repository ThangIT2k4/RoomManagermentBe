using Property.Application.Dtos;
using RoomManagerment.Shared.Messaging;

namespace Property.Application.Features.Amenities.UpsertAmenity;

public sealed record UpsertAmenityCommand(Guid OrganizationId, Guid UserId, UpsertAmenityRequest Request)
    : IAppRequest<AmenityDto>;
