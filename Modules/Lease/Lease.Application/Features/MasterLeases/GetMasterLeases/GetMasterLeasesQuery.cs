using Lease.Application.Dtos;
using RoomManagerment.Shared.Messaging;

namespace Lease.Application.Features.MasterLeases.GetMasterLeases;

public sealed record GetMasterLeasesQuery(Guid OrganizationId) : IAppRequest<IReadOnlyList<MasterLeaseDto>>;
