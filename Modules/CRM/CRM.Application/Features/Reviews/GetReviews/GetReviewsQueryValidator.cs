using FluentValidation;

namespace CRM.Application.Features.Reviews.GetReviews;

public sealed class GetReviewsQueryValidator : AbstractValidator<GetReviewsQuery>
{
    public GetReviewsQueryValidator()
    {
        RuleFor(x => x.OrganizationId)
            .NotEmpty()
            .WithMessage("Mã tổ chức không được để trống.");

        RuleFor(x => x.LeadId)
            .NotEmpty()
            .WithMessage("Mã khách hàng tiềm năng không được để trống.")
            .When(x => x.LeadId.HasValue);

        RuleFor(x => x.ViewingId)
            .NotEmpty()
            .WithMessage("Mã lịch hẹn không được để trống.")
            .When(x => x.ViewingId.HasValue);

        RuleFor(x => x.Paging!)
            .SetValidator(new CRM.Application.Features.Shared.PagingRequestValidator())
            .When(x => x.Paging is not null);
    }
}

