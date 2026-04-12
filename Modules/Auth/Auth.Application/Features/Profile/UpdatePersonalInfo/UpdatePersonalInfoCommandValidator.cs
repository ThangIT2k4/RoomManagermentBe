using Auth.Application.Validation;
using FluentValidation;

namespace Auth.Application.Features.Auth.Profile.UpdatePersonalInfo;

public sealed class UpdatePersonalInfoCommandValidator : AbstractValidator<UpdatePersonalInfoCommand>
{
    public UpdatePersonalInfoCommandValidator()
    {
        RuleFor(x => x.UserId)
            .NotEmpty()
            .WithMessage("Mã người dùng không được để trống.");

        RuleFor(x => x.FullName)
            .MaximumLength(120)
            .WithMessage("Họ và tên không được vượt quá 120 ký tự.")
            .Must(ValidationGuards.BeSafeText)
            .WithMessage("Họ và tên chứa nội dung không an toàn.")
            .When(x => !string.IsNullOrWhiteSpace(x.FullName));

        RuleFor(x => x.Address)
            .MaximumLength(255)
            .WithMessage("Địa chỉ không được vượt quá 255 ký tự.")
            .Must(ValidationGuards.BeSafeText)
            .WithMessage("Địa chỉ chứa nội dung không an toàn.")
            .When(x => !string.IsNullOrWhiteSpace(x.Address));

        RuleFor(x => x.Note)
            .MaximumLength(500)
            .WithMessage("Ghi chú không được vượt quá 500 ký tự.")
            .Must(ValidationGuards.BeSafeText)
            .WithMessage("Ghi chú chứa nội dung không an toàn.")
            .When(x => !string.IsNullOrWhiteSpace(x.Note));
    }
}
