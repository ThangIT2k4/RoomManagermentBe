using Lease.Application.Dtos;
using RoomManagerment.Shared.Messaging;

namespace Lease.Application.Features.ServiceSets.GetServiceSets;

public sealed record GetServiceSetsQuery(Guid OrganizationId) : IAppRequest<IReadOnlyList<LeaseServiceSetDto>>;
