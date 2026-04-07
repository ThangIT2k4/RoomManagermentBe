using Organization.Domain.Enums;
using Organization.Domain.Events;
using Organization.Domain.Exceptions;

namespace Organization.Domain.Entities;

public sealed class OrganizationEntity
{
    private readonly List<object> _domainEvents = [];

    public Guid Id { get; }
    public string Name { get; private set; }
    public string? Code { get; }
    public OrganizationStatus Status { get; private set; }
    public bool HasEverPaid { get; private set; }
    public string? Email { get; private set; }
    public string? Phone { get; private set; }
    public string? Mail { get; private set; }
    public string? TaxCode { get; private set; }
    public string? Address { get; private set; }
    public DateTime CreatedAt { get; }
    public DateTime? UpdatedAt { get; private set; }
    public IReadOnlyList<object> DomainEvents => _domainEvents;

    private OrganizationEntity(
        Guid id,
        string name,
        string? code,
        OrganizationStatus status,
        bool hasEverPaid,
        DateTime createdAt,
        DateTime? updatedAt)
    {
        Id = id;
        Name = name;
        Code = code;
        Status = status;
        HasEverPaid = hasEverPaid;
        CreatedAt = createdAt;
        UpdatedAt = updatedAt;
    }

    public static OrganizationEntity Create(string name, string? code = null, DateTime? now = null)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            throw new OrganizationDomainException("Organization name is required.");
        }

        var entity = new OrganizationEntity(
            Guid.NewGuid(),
            name.Trim(),
            code?.Trim(),
            OrganizationStatus.Active,
            false,
            now ?? DateTime.UtcNow,
            null);

        entity._domainEvents.Add(new OrganizationCreatedEvent(entity.Id, entity.CreatedAt));
        return entity;
    }

    public void UpdateProfile(string name, string? phone, string? email, string? mail, string? taxCode, string? address, DateTime now)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            throw new OrganizationDomainException("Organization name is required.");
        }

        Name = name.Trim();
        Phone = phone?.Trim();
        Email = email?.Trim();
        Mail = mail?.Trim();
        TaxCode = taxCode?.Trim();
        Address = address?.Trim();
        UpdatedAt = now;
        _domainEvents.Add(new OrganizationUpdatedEvent(Id, now));
    }

    public static OrganizationEntity FromPersistence(
        Guid id,
        string name,
        string? code,
        short status,
        bool hasEverPaid,
        DateTime createdAt,
        DateTime? updatedAt,
        string? email = null,
        string? phone = null,
        string? mail = null,
        string? taxCode = null,
        string? address = null)
    {
        var entity = new OrganizationEntity(id, name ?? string.Empty, code, (OrganizationStatus)status, hasEverPaid, createdAt, updatedAt)
        {
            Email = email,
            Phone = phone,
            Mail = mail,
            TaxCode = taxCode,
            Address = address
        };

        return entity;
    }
}

