using FluentValidation;

namespace Lease.Application.Features.ServiceSets.GetServiceSetById;

public sealed class GetServiceSetByIdQueryValidator : AbstractValidator<GetServiceSetByIdQuery>
{
    public GetServiceSetByIdQueryValidator()
    {
        RuleFor(x => x.OrganizationId)
            .NotEmpty().WithMessage("Mã tổ chức không được để trống.");

        RuleFor(x => x.ServiceSetId)
            .NotEmpty().WithMessage("Mã bộ dịch vụ không được để trống.");
    }
}
