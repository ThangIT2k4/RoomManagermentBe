using FluentValidation;

namespace Finance.Application.Features.Payments.HandleWebhook;

public sealed class HandlePaymentWebhookCommandValidator : AbstractValidator<HandlePaymentWebhookCommand>
{
    public HandlePaymentWebhookCommandValidator()
    {
        RuleFor(x => x.RawBody)
            .NotEmpty()
            .WithMessage("Nội dung webhook không được để trống.")
            .MaximumLength(200_000)
            .WithMessage("Nội dung webhook không được vượt quá 200000 ký tự.");

        RuleFor(x => x.Headers)
            .NotEmpty()
            .WithMessage("Headers webhook không được để trống.");
    }
}

