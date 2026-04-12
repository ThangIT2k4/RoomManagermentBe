using Organization.Application.Common;
using Organization.Application.Dtos;
using RoomManagerment.Shared.Messaging;

namespace Organization.Application.Features.Organizations.OnboardOrganization;

public sealed record OnboardOrganizationCommand(
    Guid OrganizationId,
    Guid ActorUserId,
    string Name,
    string? Phone,
    string? Email,
    string? TaxCode,
    string? Address) : IAppRequest<Result<OrganizationDto>>;
