using CRM.Domain.Common;
using CRM.Domain.Exceptions;
using CRM.Domain.Events;

namespace CRM.Domain.Entities;

public sealed class ReviewReplyEntity : AggregateRoot<Guid>
{
    public Guid ReviewId { get; private set; }
    public Guid UserId { get; private set; }
    public string Content { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime? UpdatedAt { get; private set; }

    private ReviewReplyEntity(
        Guid id,
        Guid reviewId,
        Guid userId,
        string content,
        DateTime createdAt,
        DateTime? updatedAt)
    {
        Id = id;
        ReviewId = reviewId;
        UserId = userId;
        Content = content;
        CreatedAt = createdAt;
        UpdatedAt = updatedAt;
    }

    public static ReviewReplyEntity Create(Guid reviewId, Guid userId, string content, DateTime? createdAt = null)
    {
        if (reviewId == Guid.Empty)
        {
            throw new DomainValidationException("ReviewId là bắt buộc.");
        }

        if (userId == Guid.Empty)
        {
            throw new DomainValidationException("UserId là bắt buộc.");
        }

        if (string.IsNullOrWhiteSpace(content))
        {
            throw new DomainValidationException("Nội dung là bắt buộc.");
        }

        return new ReviewReplyEntity(Guid.NewGuid(), reviewId, userId, content.Trim(), createdAt ?? DateTime.UtcNow, null);
    }

    public static ReviewReplyEntity Reconstitute(Guid id, Guid reviewId, Guid userId, string content, DateTime createdAt, DateTime? updatedAt)
    {
        return new ReviewReplyEntity(id, reviewId, userId, content, createdAt, updatedAt);
    }

    public void UpdateContent(string content, DateTime? updatedAt = null)
    {
        if (string.IsNullOrWhiteSpace(content))
        {
            throw new DomainValidationException("Nội dung là bắt buộc.");
        }

        Content = content.Trim();
        UpdatedAt = updatedAt ?? DateTime.UtcNow;
        AddDomainEvent(new ReviewReplyUpdatedEvent(Id, ReviewId, UserId, DateTimeOffset.UtcNow));
    }
}
