using Auth.Application.Validation;
using FluentValidation;

namespace Auth.Application.Features.Auth.VerifyOtp;

public sealed class VerifyOtpCommandValidator : AbstractValidator<VerifyOtpCommand>
{
    public VerifyOtpCommandValidator()
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

        RuleFor(x => x.Purpose)
            .IsInEnum()
            .WithMessage("Mục đích OTP không hợp lệ.");

        RuleFor(x => x.OtpCode)
            .NotEmpty()
            .WithMessage("Mã OTP không được để trống.")
            .Length(6)
            .WithMessage("Mã OTP phải gồm đúng 6 ký tự.")
            .Matches("^[0-9]{6}$")
            .WithMessage("Mã OTP phải gồm đúng 6 chữ số.");
    }
}
