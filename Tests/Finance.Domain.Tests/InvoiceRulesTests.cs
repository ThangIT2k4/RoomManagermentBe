using Finance.Domain;
using Finance.Domain.Entities;
using Xunit;

namespace Finance.Domain.Tests;

public sealed class InvoiceRulesTests
{
    [Fact]
    public void CanPublish_only_draft()
    {
        Assert.True(InvoiceRules.CanPublish(InvoiceStatuses.Draft));
        Assert.False(InvoiceRules.CanPublish(InvoiceStatuses.Sent));
    }

    [Fact]
    public void CanCancel_rejects_when_paid_amount_positive()
    {
        Assert.False(InvoiceRules.CanCancel(InvoiceStatuses.Draft, 1));
        Assert.True(InvoiceRules.CanCancel(InvoiceStatuses.Sent, 0));
    }

    [Fact]
    public void NextStatusAfterPayment_marks_paid_when_fully_covered()
    {
        Assert.Equal(InvoiceStatuses.Paid, InvoiceRules.NextStatusAfterPayment(100, 100));
        Assert.Null(InvoiceRules.NextStatusAfterPayment(100, 50));
    }
}
