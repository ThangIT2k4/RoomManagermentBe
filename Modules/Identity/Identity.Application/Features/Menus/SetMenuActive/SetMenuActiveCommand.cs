using Identity.Application.Common;
using MediatR;

namespace Identity.Application.Features.Menus.SetMenuActive;

public sealed record SetMenuActiveCommand(
    Guid MenuId,
    bool IsActive) : IRequest<Result>;

