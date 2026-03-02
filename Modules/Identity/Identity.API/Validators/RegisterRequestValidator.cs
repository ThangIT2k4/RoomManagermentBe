using FluentValidation;
using Identity.API.Requests;

namespace Identity.API.Validators;

public class RegisterRequestValidator : AbstractValidator<RegisterRequest>
{
    public RegisterRequestValidator()
    {
        RuleFor(x => x.Username)
            .NotEmpty()
            .WithMessage("Tên đăng nhập không được để trống")
            .MinimumLength(2)
            .WithMessage("Tên đăng nhập phải có ít nhất 2 ký tự")
            .MaximumLength(50)
            .WithMessage("Tên đăng nhập không được vượt quá 50 ký tự")
            .Matches(@"^[a-zA-Z0-9]+$")
            .WithMessage("Tên đăng nhập chỉ chứa chữ cái và số");

        RuleFor(x => x.Email)
            .NotEmpty()
            .WithMessage("Email không được để trống")
            .EmailAddress()
            .WithMessage("Email không hợp lệ");

        RuleFor(x => x.Password)
            .NotEmpty()
            .WithMessage("Mật khẩu không được để trống")
            .MinimumLength(6)
            .WithMessage("Mật khẩu phải có ít nhất 6 ký tự")
            .Matches(@"[A-Z]")
            .WithMessage("Mật khẩu phải chứa ít nhất một chữ cái viết hoa")
            .Matches(@"[a-z]")
            .WithMessage("Mật khẩu phải chứa ít nhất một chữ cái viết thường")
            .Matches(@"[0-9]")
            .WithMessage("Mật khẩu phải chứa ít nhất một chữ số");

        RuleFor(x => x.ConfirmPassword)
            .NotEmpty()
            .WithMessage("Xác nhận mật khẩu không được để trống")
            .Equal(x => x.Password)
            .WithMessage("Mật khẩu và xác nhận mật khẩu không khớp");
    }
}
