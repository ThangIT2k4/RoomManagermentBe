using CRM.Domain.Entities;
using CRM.Domain.Events;
using CRM.Domain.Exceptions;

namespace CRM.Domain.Tests.Entities;

public sealed class LeadEntityTests
{
    [Fact]
    public void Create_WithValidInput_ShouldSetDefaults()
    {
        var entity = LeadEntity.Create(Guid.NewGuid(), "  John Doe  ");

        Assert.Equal("John Doe", entity.FullName);
        Assert.Equal("new", entity.Status);
        Assert.Contains(entity.DomainEvents, x => x is LeadCreatedEvent);
    }

    [Fact]
    public void Create_WithEmptyOrganizationId_ShouldThrowDomainValidationException()
    {
        Assert.Throws<DomainValidationException>(() => LeadEntity.Create(Guid.Empty, "name"));
    }

    [Fact]
    public void UpdateStatus_WithNewStatus_ShouldAddStatusChangedEvent()
    {
        var entity = LeadEntity.Create(Guid.NewGuid(), "John Doe");
        entity.ClearDomainEvents();

        entity.UpdateStatus("qualified");

        Assert.Equal("qualified", entity.Status);
        Assert.Contains(entity.DomainEvents, x => x is LeadStatusChangedEvent);
    }
}

