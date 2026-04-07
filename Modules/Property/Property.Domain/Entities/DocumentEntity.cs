namespace Property.Domain.Entities;

public sealed class DocumentEntity
{
    public Guid Id { get; }
    public Guid OrganizationId { get; }
    public string OwnerType { get; }
    public Guid OwnerId { get; }
    public string FileUrl { get; }
    public string FileName { get; }
    public string MimeType { get; }
    public long FileSize { get; }
    public string? DocumentType { get; }
    public bool IsPrimary { get; }
    public int SortOrder { get; }
    public Guid UploadedBy { get; }
    public DateTime CreatedAt { get; }
    public DateTime? UpdatedAt { get; }

    private DocumentEntity(
        Guid id,
        Guid organizationId,
        string ownerType,
        Guid ownerId,
        string fileUrl,
        string fileName,
        string mimeType,
        long fileSize,
        string? documentType,
        bool isPrimary,
        int sortOrder,
        Guid uploadedBy,
        DateTime createdAt,
        DateTime? updatedAt)
    {
        Id = id;
        OrganizationId = organizationId;
        OwnerType = ownerType;
        OwnerId = ownerId;
        FileUrl = fileUrl;
        FileName = fileName;
        MimeType = mimeType;
        FileSize = fileSize;
        DocumentType = documentType;
        IsPrimary = isPrimary;
        SortOrder = sortOrder;
        UploadedBy = uploadedBy;
        CreatedAt = createdAt;
        UpdatedAt = updatedAt;
    }

    public static DocumentEntity FromPersistence(
        Guid id,
        Guid organizationId,
        string ownerType,
        Guid ownerId,
        string fileUrl,
        string fileName,
        string mimeType,
        long fileSize,
        string? documentType,
        bool isPrimary,
        int sortOrder,
        Guid uploadedBy,
        DateTime createdAt,
        DateTime? updatedAt)
        => new(id, organizationId, ownerType, ownerId, fileUrl, fileName, mimeType, fileSize, documentType, isPrimary, sortOrder, uploadedBy, createdAt, updatedAt);
}
