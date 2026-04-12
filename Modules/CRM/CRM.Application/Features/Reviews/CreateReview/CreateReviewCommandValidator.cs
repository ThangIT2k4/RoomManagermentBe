using CRM.Application.Validation;
using FluentValidation;

namespace CRM.Application.Features.Reviews.CreateReview;

public sealed class CreateReviewCommandValidator : AbstractValidator<CreateReviewCommand>
{
    public CreateReviewCommandValidator()
    {
        RuleFor(x => x.ViewingId)
            .NotEmpty()
            .WithMessage("Mã lịch hẹn không được để trống.");

        RuleFor(x => x.Rating)
            .InclusiveBetween((short)1, (short)5)
            .WithMessage("Điểm đánh giá phải từ 1 đến 5.");

        RuleFor(x => x.Content)
            .NotEmpty()
            .WithMessage("Nội dung đánh giá không được để trống.")
            .MaximumLength(2000)
            .WithMessage("Nội dung đánh giá không được vượt quá 2000 ký tự.")
            .Must(ValidationGuards.BeSafeText)
            .WithMessage("Nội dung đánh giá chứa nội dung không an toàn.");

        RuleFor(x => x.CreatedBy)
            .NotEmpty()
            .WithMessage("Mã người tạo không được để trống.");
    }
}
