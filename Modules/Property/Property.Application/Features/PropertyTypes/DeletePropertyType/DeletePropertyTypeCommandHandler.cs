using Property.Application.Services;
using RoomManagerment.Shared.Messaging;

namespace Property.Application.Features.PropertyTypes.DeletePropertyType;

public sealed class DeletePropertyTypeCommandHandler(IPropertyApplicationService service)
    : IAppRequestHandler<DeletePropertyTypeCommand, bool>
{
    public Task<bool> Handle(DeletePropertyTypeCommand request, CancellationToken cancellationToken)
        => service.DeletePropertyTypeAsync(request.OrganizationId, request.UserId, request.PropertyTypeId, cancellationToken);
}
