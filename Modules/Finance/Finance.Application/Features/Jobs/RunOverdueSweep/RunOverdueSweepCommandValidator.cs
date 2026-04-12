using FluentValidation;

namespace Finance.Application.Features.Jobs.RunOverdueSweep;

public sealed class RunOverdueSweepCommandValidator : AbstractValidator<RunOverdueSweepCommand>
{
    public RunOverdueSweepCommandValidator()
    {
        RuleFor(x => x.AsOfDate)
            .NotEmpty()
            .WithMessage("Ngày chốt quét quá hạn không được để trống.");
    }
}

