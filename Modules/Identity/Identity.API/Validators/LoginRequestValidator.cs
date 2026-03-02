using FluentValidation;
using Identity.API.Requests;

namespace Identity.API.Validators;

public class LoginRequestValidator : AbstractValidator<LoginRequest>
{
    public LoginRequestValidator()
    {
        RuleFor(x => x.UsernameOrEmail)
            .NotEmpty()
            .WithMessage("Email hoặc tên đăng nhập không được để trống")
            .MinimumLength(2)
            .WithMessage("Email hoặc tên đăng nhập phải có ít nhất 2 ký tự");

        RuleFor(x => x.Password)
            .NotEmpty()
            .WithMessage("Mật khẩu không được để trống")
            .MinimumLength(6)
            .WithMessage("Mật khẩu phải có ít nhất 6 ký tự");
    }
}


