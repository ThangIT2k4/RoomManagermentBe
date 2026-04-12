using Property.Application.Services;
using RoomManagerment.Shared.Messaging;

namespace Property.Application.Features.Staff.UnassignStaff;

public sealed class UnassignStaffCommandHandler(IPropertyApplicationService service)
    : IAppRequestHandler<UnassignStaffCommand, bool>
{
    public Task<bool> Handle(UnassignStaffCommand request, CancellationToken cancellationToken)
        => service.UnassignStaffAsync(request.OrganizationId, request.ActorUserId, request.PropertyId, request.TargetUserId, cancellationToken);
}
