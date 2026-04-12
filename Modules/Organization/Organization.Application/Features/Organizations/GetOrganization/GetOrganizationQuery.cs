using Organization.Application.Common;
using Organization.Application.Dtos;
using RoomManagerment.Shared.Messaging;

namespace Organization.Application.Features.Organizations.GetOrganization;

public sealed record GetOrganizationQuery(Guid OrganizationId) : IAppRequest<Result<OrganizationDto>>;
