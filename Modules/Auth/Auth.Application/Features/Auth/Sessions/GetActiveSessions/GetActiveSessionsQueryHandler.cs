using Auth.Application.Common;
using Auth.Application.Dtos;
using Auth.Application.Services;
using MediatR;

namespace Auth.Application.Features.Auth.Sessions.GetActiveSessions;

public sealed class GetActiveSessionsQueryHandler(IAuthApplicationService authService)
    : IRequestHandler<GetActiveSessionsQuery, Result<PagedSessionsResult>>
{
    public Task<Result<PagedSessionsResult>> Handle(GetActiveSessionsQuery request, CancellationToken cancellationToken)
        => authService.GetActiveSessionsAsync(new GetActiveSessionsRequest(request.UserId, request.PageNumber, request.PageSize), cancellationToken);
}
