using Property.Application.Services;
using RoomManagerment.Shared.Messaging;

namespace Property.Application.Features.Properties.DeleteProperty;

public sealed class DeletePropertyCommandHandler(IPropertyApplicationService service)
    : IAppRequestHandler<DeletePropertyCommand, bool>
{
    public Task<bool> Handle(DeletePropertyCommand request, CancellationToken cancellationToken)
        => service.DeletePropertyAsync(request.OrganizationId, request.UserId, request.PropertyId, cancellationToken);
}
