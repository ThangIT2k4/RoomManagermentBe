using Identity.Application.Common;
using Identity.Domain.Repositories;
using MediatR;

namespace Identity.Application.Features.Menus.SetMenuActive;

public sealed class SetMenuActiveCommandHandler(IMenuRepository menuRepository, IUnitOfWork unitOfWork)
    : IRequestHandler<SetMenuActiveCommand, Result>
{
    public async Task<Result> Handle(SetMenuActiveCommand command, CancellationToken cancellationToken = default)
    {
        var menu = await menuRepository.GetByIdAsync(command.MenuId, cancellationToken);
        if (menu is null)
        {
            return Result.Failure(
                new Error("Menu.NotFound", $"Menu with id '{command.MenuId}' was not found."));
        }

        menu.SetActive(command.IsActive, DateTime.UtcNow);

        await menuRepository.UpdateAsync(menu, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}

