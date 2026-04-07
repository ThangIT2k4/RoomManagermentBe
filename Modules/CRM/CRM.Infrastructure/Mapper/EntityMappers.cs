using DalLeadEntity = RoomManagerment.CRM.EntityClasses.LeadEntity;
using DomainLeadEntity = CRM.Domain.Entities.LeadEntity;
using DalViewingEntity = RoomManagerment.CRM.EntityClasses.ViewingEntity;
using DomainViewingEntity = CRM.Domain.Entities.ViewingEntity;
using DalBookingDepositEntity = RoomManagerment.CRM.EntityClasses.BookingDepositEntity;
using DomainBookingDepositEntity = CRM.Domain.Entities.BookingDepositEntity;
using DalReviewEntity = RoomManagerment.CRM.EntityClasses.ReviewEntity;
using DomainReviewEntity = CRM.Domain.Entities.ReviewEntity;
using DalReviewReplyEntity = RoomManagerment.CRM.EntityClasses.ReviewReplyEntity;
using DomainReviewReplyEntity = CRM.Domain.Entities.ReviewReplyEntity;
using DalCommissionPolicyEntity = RoomManagerment.CRM.EntityClasses.CommissionPolicyEntity;
using DomainCommissionPolicyEntity = CRM.Domain.Entities.CommissionPolicyEntity;
using DalCommissionEventEntity = RoomManagerment.CRM.EntityClasses.CommissionEventEntity;
using DomainCommissionEventEntity = CRM.Domain.Entities.CommissionEventEntity;

namespace CRM.Infrastructure.Mapper;

internal static class EntityMappers
{
    public static DomainLeadEntity ToDomain(this DalLeadEntity dal)
    {
        return DomainLeadEntity.FromPersistence(dal.Id, dal.OrganizationId, dal.FullName, dal.Status ?? string.Empty, dal.CreatedAt, dal.UpdatedAt);
    }

    public static DomainViewingEntity ToDomain(this DalViewingEntity dal)
    {
        return DomainViewingEntity.Reconstitute(dal.Id, dal.OrganizationId, dal.LeadId, dal.AgentId, dal.ScheduleAt, dal.Status ?? "scheduled", dal.Note, dal.CreatedAt, dal.UpdatedAt);
    }

    public static DomainBookingDepositEntity ToDomain(this DalBookingDepositEntity dal)
    {
        return DomainBookingDepositEntity.Reconstitute(dal.Id, dal.OrganizationId, dal.LeadId, dal.ViewingId, dal.Amount, dal.DepositType, dal.PaymentStatus ?? "pending", dal.CreatedAt, dal.UpdatedAt);
    }

    public static DomainReviewEntity ToDomain(this DalReviewEntity dal)
    {
        return DomainReviewEntity.Reconstitute(dal.Id, dal.OrganizationId, dal.UnitId, dal.UserId, dal.Rating, dal.Content, dal.IsPublic, dal.CreatedAt, dal.UpdatedAt);
    }

    public static DomainReviewReplyEntity ToDomain(this DalReviewReplyEntity dal)
    {
        return DomainReviewReplyEntity.Reconstitute(dal.Id, dal.ReviewId, dal.UserId, dal.Content ?? string.Empty, dal.CreatedAt, dal.UpdatedAt);
    }

    public static DomainCommissionPolicyEntity ToDomain(this DalCommissionPolicyEntity dal)
    {
        return DomainCommissionPolicyEntity.Reconstitute(dal.Id, dal.OrganizationId, dal.Title ?? string.Empty, dal.CalcType ?? "percent", dal.TriggerEvent ?? "deposit_paid", dal.IsActive, dal.CreatedAt, dal.UpdatedAt);
    }

    public static DomainCommissionEventEntity ToDomain(this DalCommissionEventEntity dal)
    {
        return DomainCommissionEventEntity.Reconstitute(dal.Id, dal.OrganizationId, dal.PolicyId, dal.AgentId, dal.CommissionTotal, dal.OccurredAt, dal.Status ?? "pending", dal.TriggerEvent, dal.CreatedAt, dal.UpdatedAt);
    }
}

