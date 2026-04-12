using Lease.Application.Dtos;
using RoomManagerment.Shared.Messaging;

namespace Lease.Application.Features.Leases.TerminateLease;

public sealed record TerminateLeaseCommand(
    Guid OrganizationId,
    Guid UserId,
    TerminateLeaseRequest Request) : IAppRequest<LeaseDto?>;
