using Lease.Application.Dtos;
using RoomManagerment.Shared.Messaging;

namespace Lease.Application.Features.Leases.SearchLeases;

public sealed record SearchLeasesQuery(
    Guid OrganizationId,
    string? Statuses,
    Guid? UnitId,
    string? Search,
    int Page,
    int PerPage) : IAppRequest<IReadOnlyList<LeaseDto>>;
