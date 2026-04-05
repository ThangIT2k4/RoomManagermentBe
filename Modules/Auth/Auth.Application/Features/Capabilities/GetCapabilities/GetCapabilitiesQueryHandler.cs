using Auth.Application.Common;
using Auth.Application.Dtos;
using Auth.Application.Services;
using MediatR;

namespace Auth.Application.Features.Auth.Capabilities.GetCapabilities;

public sealed class GetCapabilitiesQueryHandler(IAuthApplicationService authService)
    : IRequestHandler<GetCapabilitiesQuery, Result<PagedCapabilitiesResult>>
{
    public Task<Result<PagedCapabilitiesResult>> Handle(GetCapabilitiesQuery request, CancellationToken cancellationToken)
        => authService.GetCapabilitiesAsync(new GetCapabilitiesRequest(request.SearchTerm, request.PageNumber, request.PageSize), cancellationToken);
}
