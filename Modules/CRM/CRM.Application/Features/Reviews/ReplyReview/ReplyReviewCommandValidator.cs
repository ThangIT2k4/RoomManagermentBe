using CRM.Application.Validation;
using FluentValidation;

namespace CRM.Application.Features.Reviews.ReplyReview;

public sealed class ReplyReviewCommandValidator : AbstractValidator<ReplyReviewCommand>
{
    public ReplyReviewCommandValidator()
    {
        RuleFor(x => x.ReviewId)
            .NotEmpty()
            .WithMessage("Mã đánh giá không được để trống.");

        RuleFor(x => x.ReplyContent)
            .NotEmpty()
            .WithMessage("Nội dung phản hồi không được để trống.")
            .MinimumLength(10)
            .WithMessage("Nội dung phản hồi phải có ít nhất 10 ký tự.")
            .MaximumLength(2000)
            .WithMessage("Nội dung phản hồi không được vượt quá 2000 ký tự.")
            .Must(ValidationGuards.BeSafeText)
            .WithMessage("Nội dung phản hồi chứa nội dung không an toàn.");

        RuleFor(x => x.RepliedBy)
            .NotEmpty()
            .WithMessage("Mã người phản hồi không được để trống.");
    }
}
