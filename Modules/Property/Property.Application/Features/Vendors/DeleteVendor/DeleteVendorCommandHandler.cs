using Property.Application.Services;
using RoomManagerment.Shared.Messaging;

namespace Property.Application.Features.Vendors.DeleteVendor;

public sealed class DeleteVendorCommandHandler(IPropertyApplicationService service)
    : IAppRequestHandler<DeleteVendorCommand, bool>
{
    public Task<bool> Handle(DeleteVendorCommand request, CancellationToken cancellationToken)
        => service.DeleteVendorAsync(request.OrganizationId, request.UserId, request.VendorId, cancellationToken);
}
