using CRM.Application.Features.Leads;
using CRM.Application.Features.UseCases;
using CRM.Application.Services;
using MediatR;

namespace CRM.Application.Handlers;

public sealed class CreateLeadRequestHandler(ICrmApplicationService svc)
    : IRequestHandler<CreateLeadRequest, Result<LeadDto>>
{
    public Task<Result<LeadDto>> Handle(CreateLeadRequest request, CancellationToken cancellationToken)
        => svc.CreateLeadAsync(request, cancellationToken);
}

public sealed class GetLeadByIdQueryHandler(ICrmApplicationService svc)
    : IRequestHandler<GetLeadByIdQuery, Result<LeadDto>>
{
    public Task<Result<LeadDto>> Handle(GetLeadByIdQuery request, CancellationToken cancellationToken)
        => svc.GetLeadByIdAsync(request.LeadId, cancellationToken);
}

public sealed class UpdateLeadStatusRequestHandler(ICrmApplicationService svc)
    : IRequestHandler<UpdateLeadStatusRequest, Result<LeadDto>>
{
    public Task<Result<LeadDto>> Handle(UpdateLeadStatusRequest request, CancellationToken cancellationToken)
        => svc.UpdateLeadStatusAsync(request, cancellationToken);
}

public sealed class GetLeadsQueryHandler(ICrmApplicationService svc)
    : IRequestHandler<GetLeadsQuery, Result<GetLeadsResult>>
{
    public Task<Result<GetLeadsResult>> Handle(GetLeadsQuery request, CancellationToken cancellationToken)
        => svc.GetLeadsAsync(request, cancellationToken);
}

public sealed class CreateViewingCommandHandler(ICrmApplicationService svc)
    : IRequestHandler<CreateViewingCommand, Result<ViewingEntityDto>>
{
    public Task<Result<ViewingEntityDto>> Handle(CreateViewingCommand request, CancellationToken cancellationToken)
        => svc.CreateViewingAsync(request, cancellationToken);
}

public sealed class ConfirmViewingCommandHandler(ICrmApplicationService svc)
    : IRequestHandler<ConfirmViewingCommand, Result<ViewingEntityDto>>
{
    public Task<Result<ViewingEntityDto>> Handle(ConfirmViewingCommand request, CancellationToken cancellationToken)
        => svc.ConfirmViewingAsync(request.ViewingId, cancellationToken);
}

public sealed class CompleteViewingCommandHandler(ICrmApplicationService svc)
    : IRequestHandler<CompleteViewingCommand, Result<ViewingEntityDto>>
{
    public Task<Result<ViewingEntityDto>> Handle(CompleteViewingCommand request, CancellationToken cancellationToken)
        => svc.CompleteViewingAsync(request, cancellationToken);
}

public sealed class CancelViewingCommandHandler(ICrmApplicationService svc)
    : IRequestHandler<CancelViewingCommand, Result<ViewingEntityDto>>
{
    public Task<Result<ViewingEntityDto>> Handle(CancelViewingCommand request, CancellationToken cancellationToken)
        => svc.CancelViewingAsync(request, cancellationToken);
}

public sealed class GetViewingsQueryHandler(ICrmApplicationService svc)
    : IRequestHandler<GetViewingsQuery, Result<GetViewingsResult>>
{
    public Task<Result<GetViewingsResult>> Handle(GetViewingsQuery request, CancellationToken cancellationToken)
        => svc.GetViewingsAsync(request, cancellationToken);
}

public sealed class CreateBookingCommandHandler(ICrmApplicationService svc)
    : IRequestHandler<CreateBookingCommand, Result<BookingDepositDto>>
{
    public Task<Result<BookingDepositDto>> Handle(CreateBookingCommand request, CancellationToken cancellationToken)
        => svc.CreateBookingAsync(request, cancellationToken);
}

public sealed class ApproveBookingCommandHandler(ICrmApplicationService svc)
    : IRequestHandler<ApproveBookingCommand, Result<BookingDepositDto>>
{
    public Task<Result<BookingDepositDto>> Handle(ApproveBookingCommand request, CancellationToken cancellationToken)
        => svc.ApproveBookingAsync(request, cancellationToken);
}

public sealed class PayBookingCommandHandler(ICrmApplicationService svc)
    : IRequestHandler<PayBookingCommand, Result<BookingDepositDto>>
{
    public Task<Result<BookingDepositDto>> Handle(PayBookingCommand request, CancellationToken cancellationToken)
        => svc.PayBookingAsync(request, cancellationToken);
}

public sealed class GetBookingsQueryHandler(ICrmApplicationService svc)
    : IRequestHandler<GetBookingsQuery, Result<GetBookingsResult>>
{
    public Task<Result<GetBookingsResult>> Handle(GetBookingsQuery request, CancellationToken cancellationToken)
        => svc.GetBookingsAsync(request, cancellationToken);
}

public sealed class CreateReviewCommandHandler(ICrmApplicationService svc)
    : IRequestHandler<CreateReviewCommand, Result<ReviewDto>>
{
    public Task<Result<ReviewDto>> Handle(CreateReviewCommand request, CancellationToken cancellationToken)
        => svc.CreateReviewAsync(request, cancellationToken);
}

public sealed class ReplyReviewCommandHandler(ICrmApplicationService svc)
    : IRequestHandler<ReplyReviewCommand, Result<ReviewReplyDto>>
{
    public Task<Result<ReviewReplyDto>> Handle(ReplyReviewCommand request, CancellationToken cancellationToken)
        => svc.ReplyReviewAsync(request, cancellationToken);
}

public sealed class GetReviewsQueryHandler(ICrmApplicationService svc)
    : IRequestHandler<GetReviewsQuery, Result<GetReviewsResult>>
{
    public Task<Result<GetReviewsResult>> Handle(GetReviewsQuery request, CancellationToken cancellationToken)
        => svc.GetReviewsAsync(request, cancellationToken);
}

public sealed class CreateCommissionPolicyCommandHandler(ICrmApplicationService svc)
    : IRequestHandler<CreateCommissionPolicyCommand, Result<CommissionPolicyDto>>
{
    public Task<Result<CommissionPolicyDto>> Handle(CreateCommissionPolicyCommand request, CancellationToken cancellationToken)
        => svc.CreateCommissionPolicyAsync(request, cancellationToken);
}

public sealed class ApproveCommissionCommandHandler(ICrmApplicationService svc)
    : IRequestHandler<ApproveCommissionCommand, Result<CommissionEventDto>>
{
    public Task<Result<CommissionEventDto>> Handle(ApproveCommissionCommand request, CancellationToken cancellationToken)
        => svc.ApproveCommissionAsync(request, cancellationToken);
}

public sealed class GetCommissionPoliciesQueryHandler(ICrmApplicationService svc)
    : IRequestHandler<GetCommissionPoliciesQuery, Result<GetCommissionPoliciesResult>>
{
    public Task<Result<GetCommissionPoliciesResult>> Handle(GetCommissionPoliciesQuery request, CancellationToken cancellationToken)
        => svc.GetCommissionPoliciesAsync(request, cancellationToken);
}

public sealed class GetCommissionEventsQueryHandler(ICrmApplicationService svc)
    : IRequestHandler<GetCommissionEventsQuery, Result<GetCommissionEventsResult>>
{
    public Task<Result<GetCommissionEventsResult>> Handle(GetCommissionEventsQuery request, CancellationToken cancellationToken)
        => svc.GetCommissionEventsAsync(request, cancellationToken);
}
