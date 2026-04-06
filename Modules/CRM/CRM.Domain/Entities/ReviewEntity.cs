using CRM.Domain.Exceptions;

namespace CRM.Domain.Entities;

public sealed class ReviewEntity
{
    public Guid Id { get; private set; }
    public Guid OrganizationId { get; private set; }
    public Guid UnitId { get; private set; }
    public Guid UserId { get; private set; }
    public short Rating { get; private set; }
    public string? Content { get; private set; }
    public bool IsPublic { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime? UpdatedAt { get; private set; }

    private ReviewEntity(
        Guid id,
        Guid organizationId,
        Guid unitId,
        Guid userId,
        short rating,
        string? content,
        bool isPublic,
        DateTime createdAt,
        DateTime? updatedAt)
    {
        Id = id;
        OrganizationId = organizationId;
        UnitId = unitId;
        UserId = userId;
        Rating = rating;
        Content = content;
        IsPublic = isPublic;
        CreatedAt = createdAt;
        UpdatedAt = updatedAt;
    }

    public static ReviewEntity Create(
        Guid organizationId,
        Guid unitId,
        Guid userId,
        short rating,
        string? content,
        bool isPublic = true,
        DateTime? createdAt = null)
    {
        if (organizationId == Guid.Empty)
        {
            throw new DomainValidationException("OrganizationId is required.");
        }

        if (unitId == Guid.Empty)
        {
            throw new DomainValidationException("UnitId is required.");
        }

        if (userId == Guid.Empty)
        {
            throw new DomainValidationException("UserId is required.");
        }

        if (rating is < 1 or > 5)
        {
            throw new DomainValidationException("Rating must be between 1 and 5.");
        }

        return new ReviewEntity(
            Guid.NewGuid(),
            organizationId,
            unitId,
            userId,
            rating,
            content?.Trim(),
            isPublic,
            createdAt ?? DateTime.UtcNow,
            null);
    }

    public static ReviewEntity Reconstitute(
        Guid id,
        Guid organizationId,
        Guid unitId,
        Guid userId,
        short rating,
        string? content,
        bool isPublic,
        DateTime createdAt,
        DateTime? updatedAt)
    {
        return new ReviewEntity(id, organizationId, unitId, userId, rating, content, isPublic, createdAt, updatedAt);
    }
}
