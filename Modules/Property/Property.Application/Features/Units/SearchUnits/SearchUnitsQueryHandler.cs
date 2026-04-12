using Property.Application.Dtos;
using Property.Application.Services;
using RoomManagerment.Shared.Messaging;

namespace Property.Application.Features.Units.SearchUnits;

public sealed class SearchUnitsQueryHandler(IPropertyApplicationService service)
    : IAppRequestHandler<SearchUnitsQuery, IReadOnlyList<UnitDto>>
{
    public Task<IReadOnlyList<UnitDto>> Handle(SearchUnitsQuery request, CancellationToken cancellationToken)
        => service.SearchUnitsAsync(request.OrganizationId, request.PropertyId, request.Statuses, request.UnitType, request.MinRent, request.MaxRent, request.Search, request.Page, request.PerPage, cancellationToken);
}
