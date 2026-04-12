using Property.Application.Dtos;
using Property.Application.Services;
using RoomManagerment.Shared.Messaging;

namespace Property.Application.Features.Units.UpdateUnit;

public sealed class UpdateUnitCommandHandler(IPropertyApplicationService service)
    : IAppRequestHandler<UpdateUnitCommand, UnitDto?>
{
    public Task<UnitDto?> Handle(UpdateUnitCommand request, CancellationToken cancellationToken)
        => service.UpdateUnitAsync(request.OrganizationId, request.UserId, request.Request, cancellationToken);
}
