using Property.Application.Dtos;
using Property.Application.Services;
using RoomManagerment.Shared.Messaging;

namespace Property.Application.Features.Properties.UpdateProperty;

public sealed class UpdatePropertyCommandHandler(IPropertyApplicationService service)
    : IAppRequestHandler<UpdatePropertyCommand, PropertyDto?>
{
    public Task<PropertyDto?> Handle(UpdatePropertyCommand request, CancellationToken cancellationToken)
        => service.UpdatePropertyAsync(request.OrganizationId, request.UserId, request.Request, cancellationToken);
}
