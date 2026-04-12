using Lease.Application.Dtos;
using RoomManagerment.Shared.Messaging;

namespace Lease.Application.Features.ServiceSets.GetServiceSetById;

public sealed record GetServiceSetByIdQuery(Guid OrganizationId, Guid ServiceSetId) : IAppRequest<LeaseServiceSetDto?>;
