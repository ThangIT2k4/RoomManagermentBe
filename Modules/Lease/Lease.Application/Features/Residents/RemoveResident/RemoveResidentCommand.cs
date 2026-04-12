using RoomManagerment.Shared.Messaging;

namespace Lease.Application.Features.Residents.RemoveResident;

public sealed record RemoveResidentCommand(
    Guid OrganizationId,
    Guid UserId,
    Guid LeaseId,
    Guid ResidentId) : IAppRequest<bool>;
