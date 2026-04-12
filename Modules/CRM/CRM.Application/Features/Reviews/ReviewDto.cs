namespace CRM.Application.Features.Reviews;

public sealed record ReviewDto(
    Guid Id,
    Guid OrganizationId,
    Guid UnitId,
    Guid UserId,
    short Rating,
    string? Content,
    bool IsPublic,
    DateTime CreatedAt,
    DateTime? UpdatedAt);

public sealed record ReviewReplyDto(
    Guid Id,
    Guid ReviewId,
    Guid UserId,
    string Content,
    DateTime CreatedAt,
    DateTime? UpdatedAt);
