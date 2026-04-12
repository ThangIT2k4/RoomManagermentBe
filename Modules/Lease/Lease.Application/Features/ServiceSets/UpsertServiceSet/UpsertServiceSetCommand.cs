using Lease.Application.Dtos;
using RoomManagerment.Shared.Messaging;

namespace Lease.Application.Features.ServiceSets.UpsertServiceSet;

public sealed record UpsertServiceSetCommand(
    Guid OrganizationId,
    Guid UserId,
    UpsertLeaseServiceSetRequest Request) : IAppRequest<LeaseServiceSetDto>;
