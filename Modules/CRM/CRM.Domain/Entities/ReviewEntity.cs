using CRM.Domain.Common;
using CRM.Domain.Exceptions;
using CRM.Domain.Events;

namespace CRM.Domain.Entities;

public sealed class ReviewEntity : AggregateRoot<Guid>
{
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
            throw new DomainValidationException("OrganizationId là bắt buộc.");
        }

        if (unitId == Guid.Empty)
        {
            throw new DomainValidationException("UnitId là bắt buộc.");
        }

        if (userId == Guid.Empty)
        {
            throw new DomainValidationException("UserId là bắt buộc.");
        }

        if (rating is < 1 or > 5)
        {
            throw new DomainValidationException("Điểm đánh giá phải nằm trong khoảng từ 1 đến 5.");
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

    public void Update(short rating, string? content, bool? isPublic = null, DateTime? updatedAt = null)
    {
        if (rating is < 1 or > 5)
        {
            throw new DomainValidationException("Điểm đánh giá phải nằm trong khoảng từ 1 đến 5.");
        }

        Rating = rating;
        Content = content?.Trim();
        if (isPublic.HasValue)
        {
            IsPublic = isPublic.Value;
        }

        UpdatedAt = updatedAt ?? DateTime.UtcNow;
        AddDomainEvent(new ReviewUpdatedEvent(Id, OrganizationId, Rating, IsPublic, DateTimeOffset.UtcNow));
    }

    public void Hide(string? reason = null, DateTime? updatedAt = null)
    {
        IsPublic = false;
        UpdatedAt = updatedAt ?? DateTime.UtcNow;
        AddDomainEvent(new ReviewVisibilityChangedEvent(Id, OrganizationId, false, reason, DateTimeOffset.UtcNow));
    }
}
