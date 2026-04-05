using Auth.API.Requests;
using FluentValidation;

namespace Auth.API.Validators;

public sealed class AssignRoleApiRequestValidator : AbstractValidator<AssignRoleApiRequest>
{
    public AssignRoleApiRequestValidator()
    {
        RuleFor(x => x.OrganizationId)
            .NotEmpty();

        RuleFor(x => x.RoleId)
            .NotEmpty();
    }
}
