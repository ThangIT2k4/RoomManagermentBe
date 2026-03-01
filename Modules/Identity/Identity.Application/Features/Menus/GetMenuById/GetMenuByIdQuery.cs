using Identity.Application.Common;
using MediatR;

namespace Identity.Application.Features.Menus.GetMenuById;

public sealed record GetMenuByIdQuery(Guid MenuId) : IRequest<Result<MenuDto>>;

