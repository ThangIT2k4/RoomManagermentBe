using Lease.Application.Dtos;
using RoomManagerment.Shared.Messaging;

namespace Lease.Application.Features.Leases.GetTenantLeases;

public sealed record GetTenantLeasesQuery(Guid UserId) : IAppRequest<IReadOnlyList<LeaseDto>>;
