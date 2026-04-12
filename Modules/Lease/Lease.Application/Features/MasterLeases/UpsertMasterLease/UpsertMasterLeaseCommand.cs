using Lease.Application.Dtos;
using RoomManagerment.Shared.Messaging;

namespace Lease.Application.Features.MasterLeases.UpsertMasterLease;

public sealed record UpsertMasterLeaseCommand(
    Guid OrganizationId,
    Guid UserId,
    UpsertMasterLeaseRequest Request) : IAppRequest<MasterLeaseDto>;
