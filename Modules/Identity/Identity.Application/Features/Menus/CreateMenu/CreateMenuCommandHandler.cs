using Identity.Application.Common;
using Identity.Domain.Entities;
using Identity.Domain.Repositories;
using Identity.Domain.ValueObjects;

namespace Identity.Application.Features.Menus.CreateMenu;

public sealed class CreateMenuCommandHandler(IMenuRepository menuRepository, IUnitOfWork unitOfWork)
{
    public async Task<Result<CreateMenuResult>> HandleAsync(CreateMenuCommand command, CancellationToken cancellationToken = default)
    {
        var existing = await menuRepository.GetByCodeAsync(command.Code, cancellationToken);
        if (existing is not null)
        {
            return Result<CreateMenuResult>.Failure(
                new Error("Menu.DuplicateCode", "Menu code already exists."));
        }

        var code = MenuCode.Create(command.Code);
        var label = MenuLabel.Create(command.Label);
        MenuPath? path = string.IsNullOrWhiteSpace(command.Path) ? null : MenuPath.Create(command.Path);

        var menu = MenuEntity.Create(
            Guid.NewGuid(),
            code,
            label,
            command.OrderIndex,
            path,
            command.Icon,
            command.ParentId,
            command.IsActive
        );

        menu = await menuRepository.AddAsync(menu, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        var result = new CreateMenuResult(
            menu.Id,
            menu.Code.Value,
            menu.Label.Value,
            menu.OrderIndex,
            menu.ParentId,
            menu.IsActive);

        return Result<CreateMenuResult>.Success(result);
    }
}

