using FluentValidation;

namespace Lease.Application.Features.Leases.GetLeaseById;

public sealed class GetLeaseByIdQueryValidator : AbstractValidator<GetLeaseByIdQuery>
{
    public GetLeaseByIdQueryValidator()
    {
        RuleFor(x => x.OrganizationId)
            .NotEmpty().WithMessage("Mã tổ chức không được để trống.");

        RuleFor(x => x.LeaseId)
            .NotEmpty().WithMessage("Mã hợp đồng không được để trống.");
    }
}
