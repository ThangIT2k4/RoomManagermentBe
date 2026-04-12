using FluentValidation;

namespace CRM.Application.Features.Bookings.GetBookings;

public sealed class GetBookingsQueryValidator : AbstractValidator<GetBookingsQuery>
{
    public GetBookingsQueryValidator()
    {
        RuleFor(x => x.OrganizationId)
            .NotEmpty()
            .WithMessage("Mã tổ chức không được để trống.");

        RuleFor(x => x.LeadId)
            .NotEmpty()
            .WithMessage("Mã khách hàng tiềm năng không được để trống.")
            .When(x => x.LeadId.HasValue);

        RuleFor(x => x.Status)
            .MaximumLength(50)
            .WithMessage("Trạng thái không được vượt quá 50 ký tự.")
            .When(x => !string.IsNullOrWhiteSpace(x.Status));

        RuleFor(x => x.Paging!)
            .SetValidator(new CRM.Application.Features.Shared.PagingRequestValidator())
            .When(x => x.Paging is not null);
    }
}

