using Integration.Application.Common;
using Integration.Application.Dtos;
using Integration.Domain.Repositories;
using MediatR;

namespace Integration.Application.Features.Connections.GetConnections;

public sealed class GetConnectionsQueryHandler(IIntegrationConnectionRepository connectionRepository)
    : IRequestHandler<GetConnectionsQuery, Result<IReadOnlyCollection<IntegrationConnectionDto>>>
{
    public async Task<Result<IReadOnlyCollection<IntegrationConnectionDto>>> Handle(
        GetConnectionsQuery request,
        CancellationToken cancellationToken)
    {
        var items = await connectionRepository.GetByTenantAndUserAsync(
            request.TenantId,
            request.UserId,
            cancellationToken);

        var result = items
            .Select(IntegrationConnectionDto.FromEntity)
            .ToArray();

        return Result<IReadOnlyCollection<IntegrationConnectionDto>>.Success(result);
    }
}
