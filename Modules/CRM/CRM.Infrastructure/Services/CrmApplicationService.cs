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
using CRM.Application.Features.Shared;
using CRM.Application.Features.Viewings;
using CRM.Application.Features.Viewings.CancelViewing;
using CRM.Application.Features.Viewings.CompleteViewing;
using CRM.Application.Features.Viewings.CreateViewing;
using CRM.Application.Features.Viewings.GetViewings;
using CRM.Application.Services;
using CRM.Domain.Enums;
using CRM.Domain.Entities;
using CRM.Domain.Repositories;
using RoomManagerment.CRM.DatabaseSpecific;
using RoomManagerment.CRM.Linq;
using SD.LLBLGen.Pro.LinqSupportClasses;

namespace CRM.Infrastructure.Services;

public sealed class CrmApplicationService(
    ILeadRepository leadRepository,
    IViewingRepository viewingRepository,
    IBookingDepositRepository bookingRepository,
    IReviewRepository reviewRepository,
    IReviewReplyRepository reviewReplyRepository,
    ICommissionPolicyRepository commissionPolicyRepository,
    ICommissionEventRepository commissionEventRepository,
    ICrmCacheService cacheService,
    DataAccessAdapter adapter) : ICrmApplicationService
{
    public async Task<Result<LeadDto>> CreateLeadAsync(CreateLeadRequest request, CancellationToken cancellationToken = default)
    {
        if (request.OrganizationId == Guid.Empty)
        {
            return Result<LeadDto>.Failure(Error.BadRequest("CRM.Lead.Organization.Required", "Mã tổ chức là bắt buộc."));
        }

        LeadEntity entity;
        try
        {
            entity = LeadEntity.Create(request.OrganizationId, request.FullName, request.Status);
        }
        catch (ArgumentException)
        {
            return Result<LeadDto>.Failure(Error.BadRequest("CRM.Lead.InvalidInput", "Dữ liệu đầu vào của lead không hợp lệ."));
        }

        var created = await leadRepository.AddAsync(entity, cancellationToken);
        return Result<LeadDto>.Success(Map(created));
    }

    public async Task<Result<LeadDto>> GetLeadByIdAsync(Guid leadId, CancellationToken cancellationToken = default)
    {
        if (leadId == Guid.Empty)
        {
            return Result<LeadDto>.Failure(Error.BadRequest("CRM.Lead.Id.Required", "Mã lead là bắt buộc."));
        }

        var cacheKey = $"crm:lead:{leadId}";
        var cached = await cacheService.GetAsync<LeadDto>(cacheKey, cancellationToken);
        if (cached is not null)
        {
            return Result<LeadDto>.Success(cached);
        }

        var lead = await leadRepository.GetByIdAsync(leadId, cancellationToken);
        if (lead is null)
        {
            return Result<LeadDto>.Failure(Error.NotFound("CRM.Lead.NotFound", "Không tìm thấy lead."));
        }

        var dto = Map(lead);
        await cacheService.SetAsync(cacheKey, dto, TimeSpan.FromMinutes(5), cancellationToken);
        return Result<LeadDto>.Success(dto);
    }

    public async Task<Result<LeadDto>> UpdateLeadStatusAsync(UpdateLeadStatusRequest request, CancellationToken cancellationToken = default)
    {
        if (request.LeadId == Guid.Empty)
        {
            return Result<LeadDto>.Failure(Error.BadRequest("CRM.Lead.Id.Required", "Mã lead là bắt buộc."));
        }

        var lead = await leadRepository.GetByIdAsync(request.LeadId, cancellationToken);
        if (lead is null)
        {
            return Result<LeadDto>.Failure(Error.NotFound("CRM.Lead.NotFound", "Không tìm thấy lead."));
        }

        try
        {
            lead.UpdateStatus(request.Status);
        }
        catch (ArgumentException)
        {
            return Result<LeadDto>.Failure(Error.BadRequest("CRM.Lead.InvalidStatus", "Trạng thái lead không hợp lệ."));
        }

        var updated = await leadRepository.UpdateAsync(lead, cancellationToken);
        await cacheService.RemoveAsync($"crm:lead:{lead.Id}", cancellationToken);
        return Result<LeadDto>.Success(Map(updated));
    }

    public async Task<Result<ViewingEntityDto>> CreateViewingAsync(CreateViewingCommand command, CancellationToken cancellationToken = default)
    {
        var viewing = ViewingEntity.Create(command.LeadId, command.ScheduledAt, command.LeadId, command.AgentUserId, ViewingStatus.Scheduled.ToString().ToLowerInvariant(), command.Note);
        var created = await viewingRepository.AddAsync(viewing, cancellationToken);
        return Result<ViewingEntityDto>.Success(Map(created));
    }

    public async Task<Result<ViewingEntityDto>> ConfirmViewingAsync(Guid viewingId, CancellationToken cancellationToken = default)
    {
        var viewing = await viewingRepository.GetByIdAsync(viewingId, cancellationToken);
        if (viewing is null) return Result<ViewingEntityDto>.Failure(Error.NotFound("CRM.Viewing.NotFound", "Không tìm thấy lịch xem."));
        viewing.Confirm();
        await viewingRepository.UpdateAsync(viewing, cancellationToken);
        return Result<ViewingEntityDto>.Success(Map(viewing));
    }

    public async Task<Result<ViewingEntityDto>> CompleteViewingAsync(CompleteViewingCommand command, CancellationToken cancellationToken = default)
    {
        var viewing = await viewingRepository.GetByIdAsync(command.ViewingId, cancellationToken);
        if (viewing is null) return Result<ViewingEntityDto>.Failure(Error.NotFound("CRM.Viewing.NotFound", "Không tìm thấy lịch xem."));
        viewing.Complete(command.Summary);
        await viewingRepository.UpdateAsync(viewing, cancellationToken);
        return Result<ViewingEntityDto>.Success(Map(viewing));
    }

    public async Task<Result<ViewingEntityDto>> CancelViewingAsync(CancelViewingCommand command, CancellationToken cancellationToken = default)
    {
        var viewing = await viewingRepository.GetByIdAsync(command.ViewingId, cancellationToken);
        if (viewing is null) return Result<ViewingEntityDto>.Failure(Error.NotFound("CRM.Viewing.NotFound", "Không tìm thấy lịch xem."));
        viewing.Cancel(command.Reason);
        await viewingRepository.UpdateAsync(viewing, cancellationToken);
        return Result<ViewingEntityDto>.Success(Map(viewing));
    }

    public async Task<Result<BookingDepositDto>> CreateBookingAsync(CreateBookingCommand command, CancellationToken cancellationToken = default)
    {
        var booking = BookingDepositEntity.Create(command.RequestedBy, command.DepositAmount, command.LeadId, null, "partial", PaymentStatus.Pending.ToString().ToLowerInvariant());
        var created = await bookingRepository.AddAsync(booking, cancellationToken);
        return Result<BookingDepositDto>.Success(Map(created));
    }

    public async Task<Result<BookingDepositDto>> ApproveBookingAsync(ApproveBookingCommand command, CancellationToken cancellationToken = default)
    {
        var booking = await bookingRepository.GetByIdAsync(command.BookingId, cancellationToken);
        if (booking is null) return Result<BookingDepositDto>.Failure(Error.NotFound("CRM.Booking.NotFound", "Không tìm thấy booking."));
        booking.Approve(command.ApprovedAt);
        await bookingRepository.UpdateAsync(booking, cancellationToken);
        return Result<BookingDepositDto>.Success(Map(booking));
    }

    public async Task<Result<BookingDepositDto>> PayBookingAsync(PayBookingCommand command, CancellationToken cancellationToken = default)
    {
        var booking = await bookingRepository.GetByIdAsync(command.BookingId, cancellationToken);
        if (booking is null) return Result<BookingDepositDto>.Failure(Error.NotFound("CRM.Booking.NotFound", "Không tìm thấy booking."));
        booking.MarkPaid(command.PaidAt);
        await bookingRepository.UpdateAsync(booking, cancellationToken);
        return Result<BookingDepositDto>.Success(Map(booking));
    }

    public async Task<Result<ReviewDto>> CreateReviewAsync(CreateReviewCommand command, CancellationToken cancellationToken = default)
    {
        var review = ReviewEntity.Create(command.CreatedBy, command.ViewingId, command.CreatedBy, command.Rating, command.Content);
        var created = await reviewRepository.AddAsync(review, cancellationToken);
        return Result<ReviewDto>.Success(Map(created));
    }

    public async Task<Result<ReviewReplyDto>> ReplyReviewAsync(ReplyReviewCommand command, CancellationToken cancellationToken = default)
    {
        var reply = ReviewReplyEntity.Create(command.ReviewId, command.RepliedBy, command.ReplyContent);
        var created = await reviewReplyRepository.AddAsync(reply, cancellationToken);
        return Result<ReviewReplyDto>.Success(Map(created));
    }

    public async Task<Result<CommissionPolicyDto>> CreateCommissionPolicyAsync(CreateCommissionPolicyCommand command, CancellationToken cancellationToken = default)
    {
        var policy = CommissionPolicyEntity.Create(command.OrganizationId, command.Name, command.PolicyType, "deposit_paid");
        var created = await commissionPolicyRepository.AddAsync(policy, cancellationToken);
        return Result<CommissionPolicyDto>.Success(Map(created));
    }

    public async Task<Result<CommissionEventDto>> ApproveCommissionAsync(ApproveCommissionCommand command, CancellationToken cancellationToken = default)
    {
        var commissionEvent = await commissionEventRepository.GetByIdAsync(command.CommissionEventId, cancellationToken);
        if (commissionEvent is null) return Result<CommissionEventDto>.Failure(Error.NotFound("CRM.Commission.NotFound", "Không tìm thấy sự kiện hoa hồng."));
        commissionEvent.Approve(command.ApprovedAt);
        await commissionEventRepository.UpdateAsync(commissionEvent, cancellationToken);
        return Result<CommissionEventDto>.Success(Map(commissionEvent));
    }

    public async Task<Result<GetLeadsResult>> GetLeadsAsync(GetLeadsQuery query, CancellationToken cancellationToken = default)
    {
        var cacheKey = $"crm:leads:{query.OrganizationId}:{query.Search}:{query.Status}:{query.Paging?.PageNumber}:{query.Paging?.PageSize}";
        var cached = await cacheService.GetAsync<GetLeadsResult>(cacheKey, cancellationToken);
        if (cached is not null) return Result<GetLeadsResult>.Success(cached);
        var paging = (query.Paging ?? new PagingRequest()).Normalize();
        var linq = new LinqMetaData(adapter);
        var rows = await linq.Lead.Where(x => x.OrganizationId == query.OrganizationId).OrderByDescending(x => x.CreatedAt).Skip((paging.PageNumber - 1) * paging.PageSize).Take(paging.PageSize).ToListAsync(cancellationToken);
        var total = await linq.Lead.LongCountAsync(x => x.OrganizationId == query.OrganizationId, cancellationToken);
        var result = new GetLeadsResult(new PagedResult<LeadListItemDto>(rows.Select(x => new LeadListItemDto(x.Id, x.OrganizationId, x.FullName, x.Status ?? "new", x.CreatedAt)).ToList(), total, paging.PageNumber, paging.PageSize, (int)Math.Ceiling((double)total / paging.PageSize)));
        await cacheService.SetAsync(cacheKey, result, TimeSpan.FromMinutes(3), cancellationToken);
        return Result<GetLeadsResult>.Success(result);
    }

    public Task<Result<GetViewingsResult>> GetViewingsAsync(GetViewingsQuery query, CancellationToken cancellationToken = default) =>
        Task.FromResult(Result<GetViewingsResult>.Success(new GetViewingsResult(new PagedResult<ViewingListItemDto>(new List<ViewingListItemDto>(), 0, 1, 20, 0))));

    public Task<Result<GetBookingsResult>> GetBookingsAsync(GetBookingsQuery query, CancellationToken cancellationToken = default) =>
        Task.FromResult(Result<GetBookingsResult>.Success(new GetBookingsResult(new PagedResult<BookingListItemDto>(new List<BookingListItemDto>(), 0, 1, 20, 0))));

    public Task<Result<GetReviewsResult>> GetReviewsAsync(GetReviewsQuery query, CancellationToken cancellationToken = default) =>
        Task.FromResult(Result<GetReviewsResult>.Success(new GetReviewsResult(new PagedResult<ReviewListItemDto>(new List<ReviewListItemDto>(), 0, 1, 20, 0))));

    public Task<Result<GetCommissionPoliciesResult>> GetCommissionPoliciesAsync(GetCommissionPoliciesQuery query, CancellationToken cancellationToken = default) =>
        Task.FromResult(Result<GetCommissionPoliciesResult>.Success(new GetCommissionPoliciesResult(new PagedResult<CommissionPolicyListItemDto>(new List<CommissionPolicyListItemDto>(), 0, 1, 20, 0))));

    public Task<Result<GetCommissionEventsResult>> GetCommissionEventsAsync(GetCommissionEventsQuery query, CancellationToken cancellationToken = default) =>
        Task.FromResult(Result<GetCommissionEventsResult>.Success(new GetCommissionEventsResult(new PagedResult<CommissionEventListItemDto>(new List<CommissionEventListItemDto>(), 0, 1, 20, 0))));

    private static LeadDto Map(LeadEntity lead)
    {
        return new LeadDto(
            lead.Id,
            lead.OrganizationId,
            lead.FullName,
            lead.Status,
            lead.CreatedAt,
            lead.UpdatedAt);
    }

    private static ViewingEntityDto Map(ViewingEntity entity) => new(entity.Id, entity.OrganizationId, entity.LeadId, entity.AgentId, entity.ScheduleAt, entity.Status, entity.Note, entity.CreatedAt, entity.UpdatedAt);
    private static BookingDepositDto Map(BookingDepositEntity entity) => new(entity.Id, entity.OrganizationId, entity.LeadId, entity.ViewingId, entity.Amount, entity.DepositType, entity.PaymentStatus, entity.CreatedAt, entity.UpdatedAt);
    private static ReviewDto Map(ReviewEntity entity) => new(entity.Id, entity.OrganizationId, entity.UnitId, entity.UserId, entity.Rating, entity.Content, entity.IsPublic, entity.CreatedAt, entity.UpdatedAt);
    private static ReviewReplyDto Map(ReviewReplyEntity entity) => new(entity.Id, entity.ReviewId, entity.UserId, entity.Content, entity.CreatedAt, entity.UpdatedAt);
    private static CommissionPolicyDto Map(CommissionPolicyEntity entity) => new(entity.Id, entity.OrganizationId, entity.Title, entity.CalcType, entity.TriggerEvent, entity.IsActive, entity.CreatedAt, entity.UpdatedAt);
    private static CommissionEventDto Map(CommissionEventEntity entity) => new(entity.Id, entity.OrganizationId, entity.PolicyId, entity.AgentId, entity.CommissionTotal, entity.OccurredAt, entity.Status, entity.TriggerEvent, entity.CreatedAt, entity.UpdatedAt);
}
