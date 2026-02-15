using Identity.Application.Common;
using Identity.Domain.Repositories;

namespace Identity.Application.Features.Users.GetUsersPaged;

public sealed class GetUsersPagedQueryHandler(IUserRepository userRepository)
{
    public async Task<Result<PagedResponse<UserListItemDto>>> HandleAsync(
        GetUsersPagedQuery query,
        CancellationToken cancellationToken = default)
    {
        var paged = await userRepository.GetPagedAsync(query.Page, query.PageSize, query.Filter, cancellationToken);

        var items = paged.Items
            .Select(user => new UserListItemDto(
                user.Id,
                user.Username.Value,
                user.Email.Value,
                user.Status.ToString()))
            .ToList();

        var response = new PagedResponse<UserListItemDto>(
            items,
            paged.TotalCount,
            paged.Page,
            paged.PageSize,
            paged.TotalPages,
            paged.HasPreviousPage,
            paged.HasNextPage);

        return Result<PagedResponse<UserListItemDto>>.Success(response);
    }
}

