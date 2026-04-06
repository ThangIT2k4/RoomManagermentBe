using CRM.Domain.Entities;
using CRM.Domain.Events;

namespace CRM.Domain.Tests.Events;

public sealed class LeadCreatedEventTests
{
    [Fact]
    public void Create_ShouldRaiseLeadCreatedEvent()
    {
        var lead = LeadEntity.Create(Guid.NewGuid(), "Alice");

        var createdEvent = Assert.Single(lead.DomainEvents.OfType<LeadCreatedEvent>());
        Assert.Equal(lead.Id, createdEvent.LeadId);
        Assert.Equal(lead.OrganizationId, createdEvent.OrganizationId);
        Assert.Equal(lead.Status, createdEvent.Status);
    }
}
