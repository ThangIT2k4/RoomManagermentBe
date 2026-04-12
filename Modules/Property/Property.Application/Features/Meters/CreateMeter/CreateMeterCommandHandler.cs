using Property.Application.Dtos;
using Property.Application.Services;
using RoomManagerment.Shared.Messaging;

namespace Property.Application.Features.Meters.CreateMeter;

public sealed class CreateMeterCommandHandler(IPropertyApplicationService service)
    : IAppRequestHandler<CreateMeterCommand, MeterDto>
{
    public Task<MeterDto> Handle(CreateMeterCommand request, CancellationToken cancellationToken)
        => service.CreateMeterAsync(request.OrganizationId, request.UserId, request.Request, cancellationToken);
}
