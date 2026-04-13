using FluentValidation;

namespace Lease.Application.Features.ServiceSets.UpsertServiceSet;

public sealed class UpsertServiceSetCommandValidator : AbstractValidator<UpsertServiceSetCommand>
{
    public UpsertServiceSetCommandValidator()
    {
        RuleFor(x => x.OrganizationId)
            .NotEmpty().WithMessage("Mã tổ chức không được để trống.");

        RuleFor(x => x.UserId)
            .NotEmpty().WithMessage("Mã người dùng không được để trống.");

        RuleFor(x => x.Request.Name)
            .NotEmpty().WithMessage("Tên bộ dịch vụ không được để trống.")
            .MaximumLength(255).WithMessage("Tên bộ dịch vụ không được vượt quá 255 ký tự.");

        RuleFor(x => x.Request.Description)
            .MaximumLength(2000)
            .When(x => !string.IsNullOrWhiteSpace(x.Request.Description))
            .WithMessage("Mô tả không được vượt quá 2000 ký tự.");

        RuleFor(x => x.Request.Items)
            .NotEmpty().WithMessage("Bộ dịch vụ phải có ít nhất một item.");

        RuleForEach(x => x.Request.Items).ChildRules(item =>
        {
            item.RuleFor(i => i.ServiceId)
                .NotEmpty().WithMessage("Mã dịch vụ không được để trống.");

            item.RuleFor(i => i.Price)
                .GreaterThanOrEqualTo(0).WithMessage("Giá dịch vụ không được âm.");
        });

        RuleFor(x => x.Request.Items)
            .Must(NoDuplicateService)
            .WithMessage("Không được trùng service_id trong cùng bộ dịch vụ.");
    }

    private static bool NoDuplicateService(IReadOnlyList<Lease.Application.Dtos.LeaseServiceSetItemWriteDto>? items)
    {
        if (items is null || items.Count == 0)
        {
            return true;
        }

        return items.Select(x => x.ServiceId).Distinct().Count() == items.Count;
    }
}
