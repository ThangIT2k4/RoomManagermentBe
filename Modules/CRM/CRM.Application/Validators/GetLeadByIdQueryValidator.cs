using CRM.Application.Features.UseCases;
using FluentValidation;

namespace CRM.Application.Validators;

public sealed class GetLeadByIdQueryValidator : AbstractValidator<GetLeadByIdQuery>
{
    public GetLeadByIdQueryValidator()
    {
        RuleFor(x => x.LeadId).NotEmpty();
    }
}
