using Property.Application.Services;
using RoomManagerment.Shared.Messaging;

namespace Property.Application.Features.Units.DeleteUnit;

public sealed class DeleteUnitCommandHandler(IPropertyApplicationService service)
    : IAppRequestHandler<DeleteUnitCommand, bool>
{
    public Task<bool> Handle(DeleteUnitCommand request, CancellationToken cancellationToken)
        => service.DeleteUnitAsync(request.OrganizationId, request.UserId, request.UnitId, cancellationToken);
}
