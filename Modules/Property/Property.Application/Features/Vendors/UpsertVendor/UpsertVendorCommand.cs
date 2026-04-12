using Property.Application.Dtos;
using RoomManagerment.Shared.Messaging;

namespace Property.Application.Features.Vendors.UpsertVendor;

public sealed record UpsertVendorCommand(Guid OrganizationId, Guid UserId, UpsertVendorRequest Request)
    : IAppRequest<VendorDto>;
