using Property.Application.Dtos;
using RoomManagerment.Shared.Messaging;

namespace Property.Application.Features.Amenities.GetAmenities;

public sealed record GetAmenitiesQuery(string? Category) : IAppRequest<IReadOnlyList<AmenityDto>>;
