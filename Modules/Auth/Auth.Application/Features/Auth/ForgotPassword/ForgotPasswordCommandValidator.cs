using Auth.Application.Validation;
using FluentValidation;

namespace Auth.Application.Features.Auth.ForgotPassword;

public sealed class ForgotPasswordCommandValidator : AbstractValidator<ForgotPasswordCommand>
{
    public ForgotPasswordCommandValidator()
    {
        RuleFor(x => x.Email)
            .NotEmpty()
            .WithMessage("Email không được để trống.")
            .EmailAddress()
            .WithMessage("Email không hợp lệ.")
            .MaximumLength(120)
            .WithMessage("Email không được vượt quá 120 ký tự.")
            .Must(ValidationGuards.BeSafeText)
            .WithMessage("Email chứa nội dung không an toàn.");
    }
}
