using Auth.Application.Dtos;
using Auth.Application.Services;
using Auth.Domain.Common;
using RoomManagerment.Shared.Messaging;

namespace Auth.Application.Features.Auth.Users.GetUsers;

public sealed class GetUsersQueryHandler(IAuthApplicationService authService)
    : IAppRequestHandler<GetUsersQuery, Result<PagedUsersResult>>
{
    public Task<Result<PagedUsersResult>> Handle(GetUsersQuery request, CancellationToken cancellationToken)
    {
        var paging = PagingInput.Create(request.PageNumber, request.PageSize);
        var safeSearchTerm = SearchInput.Normalize(request.SearchTerm);
        return authService.GetUsersAsync(
            new GetUsersRequest(safeSearchTerm, paging.PageNumber, paging.PageSize, request.IncludeDeleted),
            cancellationToken);
    }
}
