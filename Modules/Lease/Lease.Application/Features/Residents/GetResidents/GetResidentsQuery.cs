using Lease.Application.Dtos;
using RoomManagerment.Shared.Messaging;

namespace Lease.Application.Features.Residents.GetResidents;

public sealed record GetResidentsQuery(Guid OrganizationId, Guid LeaseId) : IAppRequest<IReadOnlyList<LeaseResidentDto>>;
