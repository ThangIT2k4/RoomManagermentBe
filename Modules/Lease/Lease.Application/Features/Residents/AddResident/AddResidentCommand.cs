using Lease.Application.Dtos;
using RoomManagerment.Shared.Messaging;

namespace Lease.Application.Features.Residents.AddResident;

public sealed record AddResidentCommand(
    Guid OrganizationId,
    Guid UserId,
    AddResidentRequest Request) : IAppRequest<LeaseResidentDto>;
