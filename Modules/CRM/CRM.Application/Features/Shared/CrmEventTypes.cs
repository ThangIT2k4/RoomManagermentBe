namespace CRM.Application.Features.Shared;

public static class CrmEventTypes
{
    public const string CrmLeadCreated = "crm_lead_created";
    public const string CrmLeadAssigned = "crm_lead_assigned";
    public const string CrmLeadStatusUpdated = "crm_lead_status_updated";

    public const string CrmViewingScheduled = "crm_viewing_scheduled";
    public const string CrmViewingCheckedIn = "crm_viewing_checked_in";
    public const string CrmViewingCompleted = "crm_viewing_completed";
    public const string CrmViewingFeedbackAdded = "crm_viewing_feedback_added";

    public const string CrmBookingCreated = "crm_booking_created";
    public const string CrmBookingPaid = "crm_booking_paid";
    public const string CrmBookingApproved = "crm_booking_approved";
    public const string CrmBookingCancelled = "crm_booking_cancelled";
    public const string CrmBookingExpired = "crm_booking_expired";

    public const string CrmReviewCreated = "crm_review_created";
    public const string CrmReviewReplied = "crm_review_replied";

    public const string CrmCommissionGenerated = "crm_commission_generated";
    public const string CrmCommissionPaid = "crm_commission_paid";

    public static readonly IReadOnlyList<string> All =
    [
        CrmLeadCreated,
        CrmLeadAssigned,
        CrmLeadStatusUpdated,
        CrmViewingScheduled,
        CrmViewingCheckedIn,
        CrmViewingCompleted,
        CrmViewingFeedbackAdded,
        CrmBookingCreated,
        CrmBookingPaid,
        CrmBookingApproved,
        CrmBookingCancelled,
        CrmBookingExpired,
        CrmReviewCreated,
        CrmReviewReplied,
        CrmCommissionGenerated,
        CrmCommissionPaid
    ];
}
