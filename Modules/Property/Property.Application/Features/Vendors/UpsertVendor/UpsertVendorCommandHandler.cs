using Property.Application.Dtos;
using Property.Application.Services;
using RoomManagerment.Shared.Messaging;

namespace Property.Application.Features.Vendors.UpsertVendor;

public sealed class UpsertVendorCommandHandler(IPropertyApplicationService service)
    : IAppRequestHandler<UpsertVendorCommand, VendorDto>
{
    public Task<VendorDto> Handle(UpsertVendorCommand request, CancellationToken cancellationToken)
        => service.UpsertVendorAsync(request.OrganizationId, request.UserId, request.Request, cancellationToken);
}
