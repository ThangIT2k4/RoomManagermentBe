using Property.Application.Services;
using RoomManagerment.Shared.Messaging;

namespace Property.Application.Features.Amenities.DeleteAmenity;

public sealed class DeleteAmenityCommandHandler(IPropertyApplicationService service)
    : IAppRequestHandler<DeleteAmenityCommand, bool>
{
    public Task<bool> Handle(DeleteAmenityCommand request, CancellationToken cancellationToken)
        => service.DeleteAmenityAsync(request.UserId, request.AmenityId, cancellationToken);
}
