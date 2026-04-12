using Auth.Application.Validation;
using FluentValidation;

namespace Auth.Application.Features.Auth.Login;

public sealed class LoginCommandValidator : AbstractValidator<LoginCommand>
{
    public LoginCommandValidator()
    {
        RuleFor(x => x.Login)
            .NotEmpty()
            .WithMessage("Email hoặc tên đăng nhập không được để trống.")
            .MinimumLength(2)
            .WithMessage("Email hoặc tên đăng nhập phải có ít nhất 2 ký tự.")
            .MaximumLength(120)
            .WithMessage("Email hoặc tên đăng nhập không được vượt quá 120 ký tự.")
            .Must(ValidationGuards.BeSafeText)
            .WithMessage("Email hoặc tên đăng nhập chứa nội dung không an toàn.");

        RuleFor(x => x.Password)
            .NotEmpty()
            .WithMessage("Mật khẩu không được để trống.")
            .MinimumLength(6)
            .WithMessage("Mật khẩu phải có ít nhất 6 ký tự.")
            .MaximumLength(128)
            .WithMessage("Mật khẩu không được vượt quá 128 ký tự.");
    }
}
