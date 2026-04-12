using Lease.Application.Dtos;
using RoomManagerment.Shared.Messaging;

namespace Lease.Application.Features.Leases.CreateFromBooking;

public sealed record CreateFromBookingCommand(
    Guid OrganizationId,
    Guid UserId,
    CreateLeaseFromBookingRequest Request) : IAppRequest<LeaseDto>;
