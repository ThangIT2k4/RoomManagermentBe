using Auth.Application.Validation;
using FluentValidation;

namespace Auth.Application.Features.Auth.Capabilities.GetCapabilities;

public sealed class GetCapabilitiesQueryValidator : AbstractValidator<GetCapabilitiesQuery>
{
    public GetCapabilitiesQueryValidator()
    {
        RuleFor(x => x.SearchTerm)
            .Must(ValidationGuards.BeSafeText)
            .WithMessage("Search term contains unsafe content.")
            .When(x => !string.IsNullOrWhiteSpace(x.SearchTerm));
    }
}
