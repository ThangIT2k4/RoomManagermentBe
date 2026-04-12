using RoomManagerment.Shared.Messaging;

namespace Property.Application.Features.Amenities.DeleteAmenity;

public sealed record DeleteAmenityCommand(Guid UserId, Guid AmenityId) : IAppRequest<bool>;
