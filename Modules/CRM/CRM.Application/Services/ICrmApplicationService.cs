using CRM.Application.Features.Bookings;
using CRM.Application.Features.Bookings.ApproveBooking;
using CRM.Application.Features.Bookings.CreateBooking;
using CRM.Application.Features.Bookings.GetBookings;
using CRM.Application.Features.Bookings.PayBooking;
using CRM.Application.Features.Commissions;
using CRM.Application.Features.Commissions.ApproveCommission;
using CRM.Application.Features.Commissions.CreateCommissionPolicy;
using CRM.Application.Features.Commissions.GetCommissionEvents;
using CRM.Application.Features.Commissions.GetCommissionPolicies;
using CRM.Application.Features.Leads;
using CRM.Application.Features.Leads.CreateLead;
using CRM.Application.Features.Leads.GetLeads;
using CRM.Application.Features.Leads.UpdateLeadStatus;
using CRM.Application.Features.Reviews;
using CRM.Application.Features.Reviews.CreateReview;
using CRM.Application.Features.Reviews.GetReviews;
using CRM.Application.Features.Reviews.ReplyReview;
using CRM.Application.Features.Viewings;
using CRM.Application.Features.Viewings.CancelViewing;
using CRM.Application.Features.Viewings.CompleteViewing;
using CRM.Application.Features.Viewings.CreateViewing;
using CRM.Application.Features.Viewings.GetViewings;

namespace CRM.Application.Services;

public interface ICrmApplicationService
{
    Task<Result<LeadDto>> CreateLeadAsync(CreateLeadRequest request, CancellationToken cancellationToken = default);
    Task<Result<LeadDto>> GetLeadByIdAsync(Guid leadId, CancellationToken cancellationToken = default);
    Task<Result<LeadDto>> UpdateLeadStatusAsync(UpdateLeadStatusRequest request, CancellationToken cancellationToken = default);
    Task<Result<GetLeadsResult>> GetLeadsAsync(GetLeadsQuery query, CancellationToken cancellationToken = default);

    Task<Result<ViewingEntityDto>> CreateViewingAsync(CreateViewingCommand command, CancellationToken cancellationToken = default);
    Task<Result<ViewingEntityDto>> ConfirmViewingAsync(Guid viewingId, CancellationToken cancellationToken = default);
    Task<Result<ViewingEntityDto>> CompleteViewingAsync(CompleteViewingCommand command, CancellationToken cancellationToken = default);
    Task<Result<ViewingEntityDto>> CancelViewingAsync(CancelViewingCommand command, CancellationToken cancellationToken = default);
    Task<Result<GetViewingsResult>> GetViewingsAsync(GetViewingsQuery query, CancellationToken cancellationToken = default);

    Task<Result<BookingDepositDto>> CreateBookingAsync(CreateBookingCommand command, CancellationToken cancellationToken = default);
    Task<Result<BookingDepositDto>> ApproveBookingAsync(ApproveBookingCommand command, CancellationToken cancellationToken = default);
    Task<Result<BookingDepositDto>> PayBookingAsync(PayBookingCommand command, CancellationToken cancellationToken = default);
    Task<Result<GetBookingsResult>> GetBookingsAsync(GetBookingsQuery query, CancellationToken cancellationToken = default);

    Task<Result<ReviewDto>> CreateReviewAsync(CreateReviewCommand command, CancellationToken cancellationToken = default);
    Task<Result<ReviewReplyDto>> ReplyReviewAsync(ReplyReviewCommand command, CancellationToken cancellationToken = default);
    Task<Result<GetReviewsResult>> GetReviewsAsync(GetReviewsQuery query, CancellationToken cancellationToken = default);

    Task<Result<CommissionPolicyDto>> CreateCommissionPolicyAsync(CreateCommissionPolicyCommand command, CancellationToken cancellationToken = default);
    Task<Result<CommissionEventDto>> ApproveCommissionAsync(ApproveCommissionCommand command, CancellationToken cancellationToken = default);
    Task<Result<GetCommissionPoliciesResult>> GetCommissionPoliciesAsync(GetCommissionPoliciesQuery query, CancellationToken cancellationToken = default);
    Task<Result<GetCommissionEventsResult>> GetCommissionEventsAsync(GetCommissionEventsQuery query, CancellationToken cancellationToken = default);
}
