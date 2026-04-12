using Auth.Application.Validation;
using FluentValidation;

namespace Auth.Application.Features.Auth.Users.GetUsers;

public sealed class GetUsersQueryValidator : AbstractValidator<GetUsersQuery>
{
    public GetUsersQueryValidator()
    {
        RuleFor(x => x.SearchTerm)
            .Must(ValidationGuards.BeSafeText)
            .WithMessage("Từ khóa tìm kiếm chứa nội dung không an toàn.")
            .When(x => !string.IsNullOrWhiteSpace(x.SearchTerm));
    }
}
