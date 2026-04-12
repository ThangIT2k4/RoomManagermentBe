using Auth.Application.Validation;
using FluentValidation;

namespace Auth.Application.Features.Auth.Capabilities.GetCapabilities;

public sealed class GetCapabilitiesQueryValidator : AbstractValidator<GetCapabilitiesQuery>
{
    public GetCapabilitiesQueryValidator()
    {
        RuleFor(x => x.SearchTerm)
            .Must(ValidationGuards.BeSafeText)
            .WithMessage("Từ khóa tìm kiếm chứa nội dung không an toàn.")
            .When(x => !string.IsNullOrWhiteSpace(x.SearchTerm));
    }
}
