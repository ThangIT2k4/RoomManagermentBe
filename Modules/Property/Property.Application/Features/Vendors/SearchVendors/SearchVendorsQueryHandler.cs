using Property.Application.Dtos;
using Property.Application.Services;
using RoomManagerment.Shared.Messaging;

namespace Property.Application.Features.Vendors.SearchVendors;

public sealed class SearchVendorsQueryHandler(IPropertyApplicationService service)
    : IAppRequestHandler<SearchVendorsQuery, IReadOnlyList<VendorDto>>
{
    public Task<IReadOnlyList<VendorDto>> Handle(SearchVendorsQuery request, CancellationToken cancellationToken)
        => service.SearchVendorsAsync(request.OrganizationId, request.VendorType, request.Search, request.Page, request.PerPage, cancellationToken);
}
