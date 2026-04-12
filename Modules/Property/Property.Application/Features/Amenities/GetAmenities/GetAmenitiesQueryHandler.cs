using Property.Application.Dtos;
using Property.Application.Services;
using RoomManagerment.Shared.Messaging;

namespace Property.Application.Features.Amenities.GetAmenities;

public sealed class GetAmenitiesQueryHandler(IPropertyApplicationService service)
    : IAppRequestHandler<GetAmenitiesQuery, IReadOnlyList<AmenityDto>>
{
    public Task<IReadOnlyList<AmenityDto>> Handle(GetAmenitiesQuery request, CancellationToken cancellationToken)
        => service.GetAmenitiesAsync(request.Category, cancellationToken);
}
