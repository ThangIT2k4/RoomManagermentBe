namespace Property.Domain.Entities;

public sealed class VendorEntity
{
    public Guid Id { get; }
    public Guid OrganizationId { get; }
    public string Name { get; }
    public string? VendorType { get; }
    public string? Phone { get; }
    public string? Email { get; }
    public short Status { get; }
    public DateTime CreatedAt { get; }
    public DateTime? UpdatedAt { get; }

    private VendorEntity(
        Guid id,
        Guid organizationId,
        string name,
        string? vendorType,
        string? phone,
        string? email,
        short status,
        DateTime createdAt,
        DateTime? updatedAt)
    {
        Id = id;
        OrganizationId = organizationId;
        Name = name;
        VendorType = vendorType;
        Phone = phone;
        Email = email;
        Status = status;
        CreatedAt = createdAt;
        UpdatedAt = updatedAt;
    }

    public static VendorEntity FromPersistence(
        Guid id,
        Guid organizationId,
        string name,
        string? vendorType,
        string? phone,
        string? email,
        short status,
        DateTime createdAt,
        DateTime? updatedAt)
        => new(id, organizationId, name, vendorType, phone, email, status, createdAt, updatedAt);
}
