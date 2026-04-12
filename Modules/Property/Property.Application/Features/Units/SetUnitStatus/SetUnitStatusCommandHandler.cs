using Property.Application.Services;
using RoomManagerment.Shared.Messaging;

namespace Property.Application.Features.Units.SetUnitStatus;

public sealed class SetUnitStatusCommandHandler(IPropertyApplicationService service)
    : IAppRequestHandler<SetUnitStatusCommand, bool>
{
    public Task<bool> Handle(SetUnitStatusCommand request, CancellationToken cancellationToken)
        => service.SetUnitStatusAsync(request.OrganizationId, request.UserId, request.UnitId, request.Status, request.Reason, cancellationToken);
}
