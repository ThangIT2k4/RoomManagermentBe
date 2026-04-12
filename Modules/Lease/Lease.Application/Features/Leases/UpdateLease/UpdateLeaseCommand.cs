using Lease.Application.Dtos;
using RoomManagerment.Shared.Messaging;

namespace Lease.Application.Features.Leases.UpdateLease;

public sealed record UpdateLeaseCommand(
    Guid OrganizationId,
    Guid UserId,
    UpdateLeaseRequest Request) : IAppRequest<LeaseDto?>;
