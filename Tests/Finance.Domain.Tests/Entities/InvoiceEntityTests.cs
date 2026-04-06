using Finance.Domain;
using Finance.Domain.Entities;

namespace Finance.Domain.Tests.Entities;

public sealed class InvoiceEntityTests
{
    [Fact]
    public void CreateDraft_WithValidInput_ShouldCreateDraft()
    {
        var org = Guid.NewGuid();
        var lease = Guid.NewGuid();
        var entity = InvoiceEntity.CreateDraft(
            org,
            lease,
            new DateOnly(2026, 1, 1),
            new DateOnly(2026, 1, 10),
            1000m,
            notes: null,
            createdBy: null,
            isAutoCreated: false);

        Assert.Equal(1000m, entity.TotalAmount);
        Assert.Equal(InvoiceStatuses.Draft, entity.Status);
        Assert.Equal(org, entity.OrganizationId);
        Assert.Equal(lease, entity.LeaseId);
    }

    [Fact]
    public void CreateDraft_WithEmptyOrganizationId_ShouldThrowArgumentException()
    {
        Assert.Throws<ArgumentException>(() =>
            InvoiceEntity.CreateDraft(
                Guid.Empty,
                Guid.NewGuid(),
                new DateOnly(2026, 1, 1),
                new DateOnly(2026, 1, 10),
                1m,
                null,
                null,
                false));
    }

    [Fact]
    public void CreateDraft_throws_when_due_before_invoice_date()
    {
        Assert.Throws<ArgumentException>(() =>
            InvoiceEntity.CreateDraft(
                Guid.NewGuid(),
                Guid.NewGuid(),
                new DateOnly(2026, 2, 10),
                new DateOnly(2026, 2, 1),
                10,
                null,
                null,
                false));
    }

    [Fact]
    public void Publish_and_ApplyPayment_marks_paid()
    {
        var inv = InvoiceEntity.CreateDraft(
            Guid.NewGuid(),
            Guid.NewGuid(),
            new DateOnly(2026, 1, 1),
            new DateOnly(2026, 1, 15),
            100,
            null,
            null,
            false);
        inv.Publish();
        inv.ApplyPayment(100);
        Assert.Equal(InvoiceStatuses.Paid, inv.Status);
        Assert.Equal(100, inv.PaidAmount);
    }
}
