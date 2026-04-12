using FluentValidation;

namespace CRM.Application.Features.Leads.GetLeadById;

public sealed class GetLeadByIdQueryValidator : AbstractValidator<GetLeadByIdQuery>
{
    public GetLeadByIdQueryValidator()
    {
        RuleFor(x => x.LeadId)
            .NotEmpty()
            .WithMessage("Mã khách hàng tiềm năng không được để trống.");
    }
}
