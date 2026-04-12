using FluentValidation;

namespace Auth.Application.Features.Auth.Sessions.GetActiveSessions;

public sealed class GetActiveSessionsQueryValidator : AbstractValidator<GetActiveSessionsQuery>
{
    public GetActiveSessionsQueryValidator()
    {
        RuleFor(x => x.UserId).NotEmpty();

        RuleFor(x => x.PageNumber).GreaterThanOrEqualTo(1);

        RuleFor(x => x.PageSize).InclusiveBetween(1, 500);
    }
}
