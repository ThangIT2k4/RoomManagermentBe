using CRM.Application.Validation;
using FluentValidation;

namespace CRM.Application.Features.Commissions.CreateCommissionPolicy;

public sealed class CreateCommissionPolicyCommandValidator : AbstractValidator<CreateCommissionPolicyCommand>
{
    public CreateCommissionPolicyCommandValidator()
    {
        RuleFor(x => x.OrganizationId)
            .NotEmpty()
            .WithMessage("Mã tổ chức không được để trống.");

        RuleFor(x => x.Name)
            .NotEmpty()
            .WithMessage("Tên chính sách không được để trống.")
            .MaximumLength(200)
            .WithMessage("Tên chính sách không được vượt quá 200 ký tự.")
            .Must(ValidationGuards.BeSafeText)
            .WithMessage("Tên chính sách chứa nội dung không an toàn.");

        RuleFor(x => x.PolicyType)
            .NotEmpty()
            .WithMessage("Loại chính sách không được để trống.")
            .MaximumLength(50)
            .WithMessage("Loại chính sách không được vượt quá 50 ký tự.")
            .Must(ValidationGuards.BeSafeText)
            .WithMessage("Loại chính sách chứa nội dung không an toàn.");

        RuleFor(x => x.Rate)
            .GreaterThan(0)
            .WithMessage("Tỷ lệ hoa hồng phải lớn hơn 0.")
            .LessThanOrEqualTo(100)
            .WithMessage("Tỷ lệ hoa hồng không được vượt quá 100.");

        RuleFor(x => x.EffectiveFrom)
            .NotEmpty()
            .WithMessage("Ngày hiệu lực từ không được để trống.");

        RuleFor(x => x.EffectiveTo)
            .Must((cmd, effectiveTo) => !effectiveTo.HasValue || effectiveTo.Value >= cmd.EffectiveFrom)
            .WithMessage("Ngày hiệu lực đến phải lớn hơn hoặc bằng ngày hiệu lực từ.")
            .When(x => x.EffectiveTo.HasValue);
    }
}
