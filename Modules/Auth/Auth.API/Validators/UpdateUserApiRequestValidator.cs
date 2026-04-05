using Auth.API.Requests;
using FluentValidation;

namespace Auth.API.Validators;

public sealed class UpdateUserApiRequestValidator : AbstractValidator<UpdateUserApiRequest>
{
    public UpdateUserApiRequestValidator()
    {
        RuleFor(x => x.Email)
            .EmailAddress()
            .When(x => !string.IsNullOrWhiteSpace(x.Email));

        RuleFor(x => x.Username)
            .MaximumLength(50)
            .Matches("^[a-zA-Z0-9._-]+$")
            .When(x => !string.IsNullOrWhiteSpace(x.Username));

        RuleFor(x => x.Phone)
            .MaximumLength(20)
            .Matches("^[0-9+()\\-\\s]{7,20}$")
            .When(x => !string.IsNullOrWhiteSpace(x.Phone));

        RuleFor(x => x)
            .Must(x => x.Email is not null || x.Username is not null || x.Phone is not null || x.Status.HasValue)
            .WithMessage("At least one field must be provided for update.");
    }
}
