using Property.Application.Dtos;
using Property.Application.Services;
using RoomManagerment.Shared.Messaging;

namespace Property.Application.Features.PropertyTypes.UpsertPropertyType;

public sealed class UpsertPropertyTypeCommandHandler(IPropertyApplicationService service)
    : IAppRequestHandler<UpsertPropertyTypeCommand, PropertyTypeDto>
{
    public Task<PropertyTypeDto> Handle(UpsertPropertyTypeCommand request, CancellationToken cancellationToken)
        => service.UpsertPropertyTypeAsync(request.OrganizationId, request.UserId, request.Request, cancellationToken);
}
