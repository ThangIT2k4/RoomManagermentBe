using Lease.Application.Dtos;
using RoomManagerment.Shared.Messaging;

namespace Lease.Application.Features.MasterLeases.TerminateMasterLease;

public sealed record TerminateMasterLeaseCommand(
    Guid OrganizationId,
    Guid UserId,
    Guid Id,
    DateOnly TerminationDate,
    string Reason) : IAppRequest<MasterLeaseDto?>;
