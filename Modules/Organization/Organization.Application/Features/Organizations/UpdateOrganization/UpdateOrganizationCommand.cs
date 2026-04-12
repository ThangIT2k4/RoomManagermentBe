using Organization.Application.Common;
using Organization.Application.Dtos;
using RoomManagerment.Shared.Messaging;

namespace Organization.Application.Features.Organizations.UpdateOrganization;

public sealed record UpdateOrganizationCommand(
    Guid OrganizationId,
    Guid ActorUserId,
    string Name,
    string? Phone,
    string? Email,
    string? PublicMail,
    string? TaxCode,
    string? Address) : IAppRequest<Result<OrganizationDto>>;
