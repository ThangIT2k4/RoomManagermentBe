using Property.Application.Dtos;
using Property.Application.Services;
using RoomManagerment.Shared.Messaging;

namespace Property.Application.Features.PropertyTypes.GetPropertyTypes;

public sealed class GetPropertyTypesQueryHandler(IPropertyApplicationService service)
    : IAppRequestHandler<GetPropertyTypesQuery, IReadOnlyList<PropertyTypeDto>>
{
    public Task<IReadOnlyList<PropertyTypeDto>> Handle(GetPropertyTypesQuery request, CancellationToken cancellationToken)
        => service.GetPropertyTypesAsync(request.OrganizationId, cancellationToken);
}
