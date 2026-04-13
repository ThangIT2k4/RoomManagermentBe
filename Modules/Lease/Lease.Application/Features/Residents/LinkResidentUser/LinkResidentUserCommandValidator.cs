using FluentValidation;

namespace Lease.Application.Features.Residents.LinkResidentUser;

public sealed class LinkResidentUserCommandValidator : AbstractValidator<LinkResidentUserCommand>
{
    public LinkResidentUserCommandValidator()
    {
        RuleFor(x => x.OrganizationId)
            .NotEmpty().WithMessage("Mã tổ chức không được để trống.");

        RuleFor(x => x.UserId)
            .NotEmpty().WithMessage("Mã người dùng thao tác không được để trống.");

        RuleFor(x => x.Request.LeaseId)
            .NotEmpty().WithMessage("Mã hợp đồng không được để trống.");

        RuleFor(x => x.Request.ResidentId)
            .NotEmpty().WithMessage("Mã cư dân không được để trống.");

        RuleFor(x => x.Request.UserId)
            .NotEmpty().WithMessage("Mã người dùng liên kết không được để trống.");
    }
}
