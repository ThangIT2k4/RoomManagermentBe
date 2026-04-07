namespace Organization.Application.Dtos;

public sealed record OrganizationDto(
    Guid Id,
    string Name,
    string? Code,
    short Status,
    bool HasEverPaid,
    string? Email,
    string? Phone,
    string? TaxCode,
    string? Address);

public sealed record OrganizationBankingDto(
    Guid Id,
    Guid OrganizationId,
    Guid? SepayBankId,
    string AccountNumber,
    string AccountHolderName,
    string? BranchName,
    string? BranchCode,
    string? SwiftCode,
    bool IsPrimary);

public sealed record OrganizationMemberDto(
    Guid Id,
    Guid OrganizationId,
    Guid UserId,
    Guid? RoleId,
    bool IsActive,
    DateTime CreatedAt,
    DateTime? LastActiveAt,
    DateTime? LastInactiveAt);

public sealed record QuotaResultDto(
    string FeatureKey,
    int? Quota,
    int CurrentUsage,
    bool Allowed,
    int? Remaining,
    string? UpgradeUrl);

public sealed record InvitationPreviewDto(
    Guid MembershipId,
    string Email,
    string Token,
    DateTime ExpiresAt,
    bool HasAccount);

public sealed record DashboardDto(
    OrganizationDto Organization,
    IReadOnlyList<OrganizationMemberDto> Members);
