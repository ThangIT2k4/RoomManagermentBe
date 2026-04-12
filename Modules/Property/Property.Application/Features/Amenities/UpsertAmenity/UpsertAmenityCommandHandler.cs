using Property.Application.Dtos;
using Property.Application.Services;
using RoomManagerment.Shared.Messaging;

namespace Property.Application.Features.Amenities.UpsertAmenity;

public sealed class UpsertAmenityCommandHandler(IPropertyApplicationService service)
    : IAppRequestHandler<UpsertAmenityCommand, AmenityDto>
{
    public Task<AmenityDto> Handle(UpsertAmenityCommand request, CancellationToken cancellationToken)
        => service.UpsertAmenityAsync(request.OrganizationId, request.UserId, request.Request, cancellationToken);
}
