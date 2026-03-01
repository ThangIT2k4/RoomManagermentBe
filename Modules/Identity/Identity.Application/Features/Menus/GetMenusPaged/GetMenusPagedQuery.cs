using Identity.Application.Common;
using Identity.Domain.Common;
using MediatR;

namespace Identity.Application.Features.Menus.GetMenusPaged;

public sealed record GetMenusPagedQuery(
    int Page,
    int PageSize,
    QueryFilter? Filter = null) : IRequest<Result<PagedResponse<MenuPagedListItemDto>>>;
