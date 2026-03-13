using Integration.Application.Common;
using Integration.Application.Dtos;
using MediatR;

namespace Integration.Application.Features.Connections.GetConnections;

public sealed class GetConnectionsQuery : IRequest<Result<IReadOnlyCollection<IntegrationConnectionDto>>>
{
    public required string TenantId { get; init; }
    public required string UserId { get; init; }
}
