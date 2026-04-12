using FluentValidation;

namespace Auth.Application.Features.Users.UpdateUser;

public sealed class UpdateUserCommandValidator : AbstractValidator<UpdateUserCommand>
{
    public UpdateUserCommandValidator()
    {
        RuleFor(x => x.UserId)
            .NotEmpty()
            .WithMessage("Mã người dùng không được để trống.");

        RuleFor(x => x.Email)
            .EmailAddress()
            .WithMessage("Email không hợp lệ.")
            .When(x => !string.IsNullOrWhiteSpace(x.Email));

        RuleFor(x => x.Username)
            .MaximumLength(50)
            .WithMessage("Tên đăng nhập không được vượt quá 50 ký tự.")
            .Matches("^[a-zA-Z0-9._-]+$")
            .WithMessage("Tên đăng nhập chỉ được chứa chữ cái, chữ số, dấu chấm, gạch dưới và gạch ngang.")
            .When(x => !string.IsNullOrWhiteSpace(x.Username));

        RuleFor(x => x.Phone)
            .MaximumLength(20)
            .WithMessage("Số điện thoại không được vượt quá 20 ký tự.")
            .Matches("^[0-9+()\\-\\s]{7,20}$")
            .WithMessage("Số điện thoại có định dạng không hợp lệ.")
            .When(x => !string.IsNullOrWhiteSpace(x.Phone));

        RuleFor(x => x)
            .Must(x => x.Email is not null || x.Username is not null || x.Phone is not null || x.Status.HasValue)
            .WithMessage("Phải cung cấp ít nhất một trường để cập nhật.");
    }
}
