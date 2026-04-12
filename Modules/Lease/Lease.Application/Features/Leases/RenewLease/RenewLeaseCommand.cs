using Lease.Application.Dtos;
using RoomManagerment.Shared.Messaging;

namespace Lease.Application.Features.Leases.RenewLease;

public sealed record RenewLeaseCommand(
    Guid OrganizationId,
    Guid UserId,
    RenewLeaseRequest Request) : IAppRequest<LeaseDto>;
