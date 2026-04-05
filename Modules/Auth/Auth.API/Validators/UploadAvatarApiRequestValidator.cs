using Auth.API.Requests;
using FluentValidation;

namespace Auth.API.Validators;

public sealed class UploadAvatarApiRequestValidator : AbstractValidator<UploadAvatarApiRequest>
{
    public UploadAvatarApiRequestValidator()
    {
        RuleFor(x => x.AvatarUrl)
            .NotEmpty()
            .MaximumLength(500)
            .Must(url => Uri.TryCreate(url, UriKind.Absolute, out var uri) && (uri.Scheme == Uri.UriSchemeHttp || uri.Scheme == Uri.UriSchemeHttps))
            .WithMessage("AvatarUrl must be a valid http/https URL.");
    }
}
