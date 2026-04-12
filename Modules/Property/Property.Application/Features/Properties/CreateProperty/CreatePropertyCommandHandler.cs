using Property.Application.Dtos;
using Property.Application.Services;
using RoomManagerment.Shared.Messaging;

namespace Property.Application.Features.Properties.CreateProperty;

public sealed class CreatePropertyCommandHandler(IPropertyApplicationService service)
    : IAppRequestHandler<CreatePropertyCommand, PropertyDto>
{
    public Task<PropertyDto> Handle(CreatePropertyCommand request, CancellationToken cancellationToken)
        => service.CreatePropertyAsync(request.OrganizationId, request.UserId, request.Request, cancellationToken);
}
