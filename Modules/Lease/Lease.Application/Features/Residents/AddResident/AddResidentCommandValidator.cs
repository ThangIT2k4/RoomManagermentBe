using FluentValidation;

namespace Lease.Application.Features.Residents.AddResident;

public sealed class AddResidentCommandValidator : AbstractValidator<AddResidentCommand>
{
    private static readonly string[] AllowedRelationships = ["family", "roommate", "other", "primary"];

    public AddResidentCommandValidator()
    {
        RuleFor(x => x.OrganizationId)
            .NotEmpty().WithMessage("Mã tổ chức không được để trống.");

        RuleFor(x => x.UserId)
            .NotEmpty().WithMessage("Mã người dùng không được để trống.");

        RuleFor(x => x.Request.LeaseId)
            .NotEmpty().WithMessage("Mã hợp đồng không được để trống.");

        RuleFor(x => x.Request.FullName)
            .NotEmpty().WithMessage("Họ tên cư dân không được để trống.")
            .MaximumLength(255).WithMessage("Họ tên cư dân không được vượt quá 255 ký tự.");

        RuleFor(x => x.Request.Relationship)
            .Must(IsAllowedRelationship)
            .When(x => !string.IsNullOrWhiteSpace(x.Request.Relationship))
            .WithMessage("Quan hệ cư dân không hợp lệ.");

        RuleFor(x => x.Request.Phone)
            .MaximumLength(30)
            .When(x => !string.IsNullOrWhiteSpace(x.Request.Phone))
            .WithMessage("Số điện thoại không được vượt quá 30 ký tự.");

        RuleFor(x => x.Request.Email)
            .EmailAddress()
            .When(x => !string.IsNullOrWhiteSpace(x.Request.Email))
            .WithMessage("Email cư dân không hợp lệ.");
    }

    private static bool IsAllowedRelationship(string? value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return false;
        }

        var normalized = value.Trim().ToLowerInvariant();
        return Array.IndexOf(AllowedRelationships, normalized) >= 0;
    }
}
