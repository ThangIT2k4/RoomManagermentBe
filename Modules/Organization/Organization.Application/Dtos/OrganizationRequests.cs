namespace Organization.Application.Dtos;

public sealed record OnboardOrganizationRequest(
    Guid OrganizationId,
    Guid ActorUserId,
    string Name,
    string? Phone,
    string? Email,
    string? TaxCode,
    string? Address);

public sealed record UpdateOrganizationRequest(
    Guid OrganizationId,
    Guid ActorUserId,
    string Name,
    string? Phone,
    string? Email,
    string? PublicMail,
    string? TaxCode,
    string? Address);

public sealed record AddOrganizationBankingRequest(
    Guid OrganizationId,
    Guid ActorUserId,
    Guid? SepayBankId,
    string AccountNumber,
    string AccountHolderName,
    string? BranchName,
    string? BranchCode,
    string? SwiftCode,
    bool IsPrimary);

public sealed record SetPrimaryBankingRequest(Guid OrganizationId, Guid BankingId, Guid ActorUserId);
public sealed record RemoveBankingRequest(Guid OrganizationId, Guid BankingId, Guid ActorUserId);

public sealed record UpsertMemberRequest(Guid OrganizationId, Guid UserId, Guid RoleId, Guid ActorUserId);
public sealed record RemoveMemberRequest(Guid OrganizationId, Guid UserId, Guid ActorUserId, string? Reason);
public sealed record ChangeMemberRoleRequest(Guid OrganizationId, Guid UserId, Guid RoleId, Guid ActorUserId);
public sealed record ChangeMemberActivationRequest(Guid OrganizationId, Guid UserId, bool IsActive, Guid ActorUserId, string? Reason);

public sealed record SendInvitationRequest(Guid OrganizationId, string Email, Guid RoleId, Guid ActorUserId, string? Note);
public sealed record AcceptInvitationRequest(string Token, Guid UserId);
public sealed record CancelInvitationRequest(Guid OrganizationId, Guid MembershipId, Guid ActorUserId);
public sealed record ResendInvitationRequest(Guid OrganizationId, Guid MembershipId, Guid ActorUserId);

public sealed record CanMemberRequest(Guid OrganizationId, Guid UserId, string CapabilityKey);
public sealed record CheckQuotaRequest(Guid OrganizationId, string FeatureKey, int CurrentUsage);

public sealed record UpsertEmailSettingsRequest(
    Guid OrganizationId,
    Guid ActorUserId,
    string FromName,
    string FromEmail,
    string Provider,
    string? SmtpHost,
    int? SmtpPort,
    string? SmtpEncryption,
    string? SmtpUsername,
    string? SmtpPassword);
