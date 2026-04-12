using Lease.Application.Dtos;
using RoomManagerment.Shared.Messaging;

namespace Lease.Application.Features.Residents.LinkResidentUser;

public sealed record LinkResidentUserCommand(
    Guid OrganizationId,
    Guid UserId,
    LinkResidentUserRequest Request) : IAppRequest<LeaseResidentDto?>;
