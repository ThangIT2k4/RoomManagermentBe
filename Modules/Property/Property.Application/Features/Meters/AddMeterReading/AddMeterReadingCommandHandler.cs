using Property.Application.Services;
using RoomManagerment.Shared.Messaging;

namespace Property.Application.Features.Meters.AddMeterReading;

public sealed class AddMeterReadingCommandHandler(IPropertyApplicationService service)
    : IAppRequestHandler<AddMeterReadingCommand, bool>
{
    public Task<bool> Handle(AddMeterReadingCommand request, CancellationToken cancellationToken)
        => service.AddMeterReadingAsync(request.OrganizationId, request.UserId, request.Request, cancellationToken);
}
