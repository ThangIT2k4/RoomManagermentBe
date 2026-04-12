using Property.Application.Dtos;
using RoomManagerment.Shared.Messaging;

namespace Property.Application.Features.Vendors.SearchVendors;

public sealed record SearchVendorsQuery(Guid OrganizationId, string? VendorType, string? Search, int Page, int PerPage)
    : IAppRequest<IReadOnlyList<VendorDto>>;
