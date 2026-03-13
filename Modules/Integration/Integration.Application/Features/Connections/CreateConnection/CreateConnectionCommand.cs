using Integration.Application.Common;
using Integration.Application.Dtos;
using Integration.Domain.Enums;
using MediatR;

namespace Integration.Application.Features.Connections.CreateConnection;

public sealed class CreateConnectionCommand : IRequest<Result<IntegrationConnectionDto>>
{
    public required string TenantId { get; init; }
    public required string UserId { get; init; }
    public required IntegrationProvider Provider { get; init; }
    public string? AuthorizationCode { get; init; }
    public string? RedirectUri { get; init; }
}
