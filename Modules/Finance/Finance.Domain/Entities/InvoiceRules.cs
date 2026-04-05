using Finance.Domain;

namespace Finance.Domain.Entities;

public static class InvoiceRules
{
    public static bool CanPublish(string status) =>
        string.Equals(status, InvoiceStatuses.Draft, StringComparison.OrdinalIgnoreCase);

    public static bool CanEdit(string status) =>
        string.Equals(status, InvoiceStatuses.Draft, StringComparison.OrdinalIgnoreCase);

    public static bool CanCancel(string status, decimal paidAmount)
    {
        if (paidAmount > 0)
        {
            return false;
        }

        return status.Equals(InvoiceStatuses.Draft, StringComparison.OrdinalIgnoreCase)
               || status.Equals(InvoiceStatuses.Sent, StringComparison.OrdinalIgnoreCase)
               || status.Equals(InvoiceStatuses.Overdue, StringComparison.OrdinalIgnoreCase);
    }

    public static bool CanRecordPayment(string status) =>
        status.Equals(InvoiceStatuses.Sent, StringComparison.OrdinalIgnoreCase)
        || status.Equals(InvoiceStatuses.Overdue, StringComparison.OrdinalIgnoreCase);

    public static string? NextStatusAfterPayment(decimal totalAmount, decimal newPaidAmount) =>
        newPaidAmount >= totalAmount ? InvoiceStatuses.Paid : null;
}
