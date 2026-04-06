using CRM.Domain.Exceptions;

namespace CRM.Domain.Entities;

public sealed class ReviewReplyEntity
{
    public Guid Id { get; private set; }
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
            throw new DomainValidationException("ReviewId is required.");
        }

        if (userId == Guid.Empty)
        {
            throw new DomainValidationException("UserId is required.");
        }

        if (string.IsNullOrWhiteSpace(content))
        {
            throw new DomainValidationException("Content is required.");
        }

        return new ReviewReplyEntity(Guid.NewGuid(), reviewId, userId, content.Trim(), createdAt ?? DateTime.UtcNow, null);
    }

    public static ReviewReplyEntity Reconstitute(Guid id, Guid reviewId, Guid userId, string content, DateTime createdAt, DateTime? updatedAt)
    {
        return new ReviewReplyEntity(id, reviewId, userId, content, createdAt, updatedAt);
    }
}
