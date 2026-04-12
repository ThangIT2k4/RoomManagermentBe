using Lease.Application.Dtos;
using RoomManagerment.Shared.Messaging;

namespace Lease.Application.Features.ServiceSets.ApplyServiceSet;

public sealed record ApplyServiceSetCommand(
    Guid OrganizationId,
    Guid UserId,
    ApplyServiceSetRequest Request) : IAppRequest<bool>;
