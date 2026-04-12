using Property.Application.Dtos;
using Property.Application.Services;
using RoomManagerment.Shared.Messaging;

namespace Property.Application.Features.Units.CreateUnit;

public sealed class CreateUnitCommandHandler(IPropertyApplicationService service)
    : IAppRequestHandler<CreateUnitCommand, UnitDto>
{
    public Task<UnitDto> Handle(CreateUnitCommand request, CancellationToken cancellationToken)
        => service.CreateUnitAsync(request.OrganizationId, request.UserId, request.Request, cancellationToken);
}
