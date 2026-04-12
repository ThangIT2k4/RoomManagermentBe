using RoomManagerment.Shared.Messaging;

namespace Property.Application.Features.Vendors.DeleteVendor;

public sealed record DeleteVendorCommand(Guid OrganizationId, Guid UserId, Guid VendorId)
    : IAppRequest<bool>;
