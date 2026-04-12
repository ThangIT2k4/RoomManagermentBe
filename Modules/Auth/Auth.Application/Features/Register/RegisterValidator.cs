using Auth.Application.Validation;
using FluentValidation;

namespace Auth.Application.Features.Register;

public sealed class RegisterValidator : AbstractValidator<RegisterCommand>
{
    public RegisterValidator()
    {
        RuleFor(x => x.Email)
            .NotEmpty()
            .WithMessage("Email không được để trống.")
            .EmailAddress()
            .WithMessage("Email không hợp lệ.");

        RuleFor(x => x.FullName)
            .NotEmpty()
            .WithMessage("Họ và tên không được để trống.")
            .MaximumLength(200)
            .WithMessage("Họ và tên không được vượt quá 200 ký tự.")
            .Must(ValidationGuards.BeSafeText)
            .WithMessage("Họ và tên chứa nội dung không an toàn.");

        RuleFor(x => x.Password)
            .NotEmpty()
            .WithMessage("Mật khẩu không được để trống.")
            .MinimumLength(8)
            .WithMessage("Mật khẩu phải có ít nhất 8 ký tự.")
            .Matches("[A-Z]")
            .WithMessage("Mật khẩu phải chứa ít nhất một chữ cái in hoa.")
            .Matches("[a-z]")
            .WithMessage("Mật khẩu phải chứa ít nhất một chữ cái in thường.")
            .Matches("[0-9]")
            .WithMessage("Mật khẩu phải chứa ít nhất một chữ số.");

        RuleFor(x => x.Username)
            .MaximumLength(50)
            .WithMessage("Tên đăng nhập không được vượt quá 50 ký tự.")
            .Matches("^[a-zA-Z0-9._-]+$")
            .WithMessage("Tên đăng nhập chỉ được chứa chữ cái, chữ số, dấu chấm, gạch dưới và gạch ngang.")
            .Must(ValidationGuards.BeSafeText)
            .WithMessage("Tên đăng nhập chứa nội dung không an toàn.")
            .When(x => !string.IsNullOrWhiteSpace(x.Username));

        RuleFor(x => x.Phone)
            .MaximumLength(20)
            .WithMessage("Số điện thoại không được vượt quá 20 ký tự.")
            .Matches("^[0-9+()\\-\\s]{7,20}$")
            .WithMessage("Số điện thoại có định dạng không hợp lệ.")
            .When(x => !string.IsNullOrWhiteSpace(x.Phone));
    }
}
