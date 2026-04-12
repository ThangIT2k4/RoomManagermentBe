using FluentValidation;

namespace CRM.Application.Features.Shared;

public sealed class PagingRequestValidator : AbstractValidator<PagingRequest>
{
    public PagingRequestValidator()
    {
        RuleFor(x => x.PageNumber)
            .GreaterThan(0)
            .WithMessage("Số trang phải lớn hơn 0.")
            .LessThanOrEqualTo(PagingDefaults.MaxPageNumber)
            .WithMessage($"Số trang không được vượt quá {PagingDefaults.MaxPageNumber}.");

        RuleFor(x => x.PageSize)
            .InclusiveBetween(1, PagingDefaults.MaxPageSize)
            .WithMessage($"Số bản ghi mỗi trang phải từ 1 đến {PagingDefaults.MaxPageSize}.");
    }
}
