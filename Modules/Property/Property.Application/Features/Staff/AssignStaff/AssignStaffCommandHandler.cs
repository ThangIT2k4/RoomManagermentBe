using Property.Application.Dtos;
using Property.Application.Services;
using RoomManagerment.Shared.Messaging;

namespace Property.Application.Features.Staff.AssignStaff;

public sealed class AssignStaffCommandHandler(IPropertyApplicationService service)
    : IAppRequestHandler<AssignStaffCommand, PropertyStaffDto>
{
    public Task<PropertyStaffDto> Handle(AssignStaffCommand request, CancellationToken cancellationToken)
        => service.AssignStaffAsync(request.OrganizationId, request.UserId, request.Request, cancellationToken);
}
