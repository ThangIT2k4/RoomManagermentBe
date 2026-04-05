using Auth.Application.Common;
using Auth.Application.Dtos;
using Auth.Application.Services;
using MediatR;

namespace Auth.Application.Features.Auth.Users.GetUsers;

public sealed class GetUsersQueryHandler(IAuthApplicationService authService)
    : IRequestHandler<GetUsersQuery, Result<PagedUsersResult>>
{
    public Task<Result<PagedUsersResult>> Handle(GetUsersQuery request, CancellationToken cancellationToken)
        => authService.GetUsersAsync(new GetUsersRequest(request.SearchTerm, request.PageNumber, request.PageSize, request.IncludeDeleted), cancellationToken);
}
