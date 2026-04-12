using FluentValidation;

namespace Auth.Application.Features.Auth.Profile.UploadAvatar;

public sealed class UploadAvatarCommandValidator : AbstractValidator<UploadAvatarCommand>
{
    public UploadAvatarCommandValidator()
    {
        RuleFor(x => x.UserId)
            .NotEmpty()
            .WithMessage("Mã người dùng không được để trống.");

        RuleFor(x => x.AvatarUrl)
            .NotEmpty()
            .WithMessage("Đường dẫn ảnh đại diện không được để trống.")
            .MaximumLength(500)
            .WithMessage("Đường dẫn ảnh đại diện không được vượt quá 500 ký tự.")
            .Must(url => Uri.TryCreate(url, UriKind.Absolute, out var uri) && (uri.Scheme == Uri.UriSchemeHttp || uri.Scheme == Uri.UriSchemeHttps))
            .WithMessage("Đường dẫn ảnh đại diện phải là URL hợp lệ với giao thức http hoặc https.");
    }
}
