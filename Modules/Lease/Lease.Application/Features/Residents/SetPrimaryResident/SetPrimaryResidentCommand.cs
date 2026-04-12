using RoomManagerment.Shared.Messaging;

namespace Lease.Application.Features.Residents.SetPrimaryResident;

public sealed record SetPrimaryResidentCommand(
    Guid OrganizationId,
    Guid UserId,
    Guid LeaseId,
    Guid ResidentId) : IAppRequest<bool>;
