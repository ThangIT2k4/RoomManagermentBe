using Auth.Application.Validation;
using FluentValidation;

namespace Auth.Application.Features.Auth.ResetPassword;

public sealed class ResetPasswordCommandValidator : AbstractValidator<ResetPasswordCommand>
{
    public ResetPasswordCommandValidator()
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

        RuleFor(x => x.OtpCode)
            .NotEmpty()
            .WithMessage("Mã OTP không được để trống.")
            .Length(6)
            .WithMessage("Mã OTP phải gồm đúng 6 ký tự.")
            .Matches("^[0-9]{6}$")
            .WithMessage("Mã OTP phải gồm đúng 6 chữ số.");

        RuleFor(x => x.NewPassword)
            .NotEmpty()
            .WithMessage("Mật khẩu mới không được để trống.")
            .MinimumLength(8)
            .WithMessage("Mật khẩu mới phải có ít nhất 8 ký tự.")
            .MaximumLength(128)
            .WithMessage("Mật khẩu mới không được vượt quá 128 ký tự.");
    }
}
