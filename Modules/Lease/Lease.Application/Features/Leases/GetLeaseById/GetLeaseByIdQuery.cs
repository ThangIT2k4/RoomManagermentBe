using Lease.Application.Dtos;
using RoomManagerment.Shared.Messaging;

namespace Lease.Application.Features.Leases.GetLeaseById;

public sealed record GetLeaseByIdQuery(Guid OrganizationId, Guid LeaseId) : IAppRequest<LeaseDto?>;
