using Lease.Application.Dtos;
using RoomManagerment.Shared.Messaging;

namespace Lease.Application.Features.Leases.CreateManual;

public sealed record CreateManualLeaseCommand(
    Guid OrganizationId,
    Guid UserId,
    CreateManualLeaseRequest Request) : IAppRequest<LeaseDto>;
