namespace Auth.Application.Dtos;

public enum OtpPurpose
{
    VerifyEmail = 1,
    ResetPassword = 2
}

public sealed record UserDto(
    Guid Id,
    string Email,
    string? Username,
    short Status,
    DateTime CreatedAt,
    DateTime? UpdatedAt,
    DateTime? EmailVerifiedAt,
    DateTime? LastLoginAt,
    string? Phone);

public sealed record SessionDto(
    string Id,
    Guid UserId,
    string? IpAddress,
    string? UserAgent,
    DateTime LastActivity,
    DateTime? ExpiresAt,
    DateTime CreatedAt);

public sealed record UserProfileDto(
    Guid UserId,
    string? FullName,
    string? Avatar,
    DateOnly? Dob,
    short? Gender,
    string? Address,
    string? Note,
    DateTime CreatedAt,
    DateTime? UpdatedAt);

public sealed record RoleDto(Guid Id, string KeyCode, string Name, string? Description);

public sealed record CapabilityDto(Guid Id, string KeyCode, string Name, string? Description, string? Category, int DisplayOrder);

public sealed record RegisterRequest(string Email, string Password, string FullName, string? Username, string? Phone);
public sealed record LoginRequest(string Login, string Password, string? IpAddress, string? UserAgent, bool RememberMe);
public sealed record LogoutRequest(string SessionId);
public sealed record GetCurrentUserRequest(string SessionId);
public sealed record GetUserByIdRequest(Guid UserId);
public sealed record GetUsersRequest(string? SearchTerm, int PageNumber = 1, int PageSize = 20, bool IncludeDeleted = false);
public sealed record UpdateUserRequest(Guid UserId, string? Email, string? Username, string? Phone, short? Status);
public sealed record DeleteUserRequest(Guid UserId, Guid DeletedBy);

public sealed record ChangePasswordRequest(Guid UserId, string CurrentPassword, string NewPassword, string? RetainSessionId = null);
public sealed record ForgotPasswordRequest(string Email);
public sealed record ResetPasswordRequest(string Email, string OtpCode, string NewPassword);

public sealed record SendOtpRequest(string Email, OtpPurpose Purpose, Guid? UserId = null);
public sealed record VerifyOtpRequest(string Email, OtpPurpose Purpose, string OtpCode);
/// <summary>Email verification: user identity comes from the OTP record, not from the client.</summary>
public sealed record VerifyEmailRequest(string Email, string OtpCode);
public sealed record ResendOtpRequest(string Email, OtpPurpose Purpose, Guid? UserId = null);

public sealed record CreateSessionRequest(Guid UserId, string? IpAddress, string? UserAgent, bool RememberMe);
public sealed record GetActiveSessionsRequest(Guid UserId, int PageNumber = 1, int PageSize = 20);
public sealed record LogoutAllSessionsRequest(Guid UserId);

public sealed record ChangeUserStatusRequest(Guid UserId, short Status);

public sealed record GetProfileRequest(Guid UserId);
public sealed record UpdateProfileRequest(Guid UserId, string? FullName, DateOnly? Dob, short? Gender, string? Address, string? Note);
public sealed record UploadAvatarRequest(Guid UserId, string AvatarUrl);
public sealed record UpdatePersonalInfoRequest(Guid UserId, string? FullName, DateOnly? Dob, short? Gender, string? Address, string? Note);

public sealed record AssignRoleRequest(Guid OrganizationId, Guid UserId, Guid RoleId);
public sealed record RemoveRoleRequest(Guid OrganizationId, Guid UserId);
public sealed record GetUserRolesRequest(Guid UserId, Guid? OrganizationId = null);
public sealed record GetCapabilitiesRequest(string? SearchTerm = null, int PageNumber = 1, int PageSize = 50);

public sealed record LoginResult(UserDto User, SessionDto Session);
public sealed record RegisterResult(UserDto User);
public sealed record PagedUsersResult(IReadOnlyList<UserDto> Items, long TotalCount, int PageNumber, int PageSize, int TotalPages);
public sealed record PagedSessionsResult(IReadOnlyList<SessionDto> Items, long TotalCount, int PageNumber, int PageSize, int TotalPages);
public sealed record PagedCapabilitiesResult(IReadOnlyList<CapabilityDto> Items, long TotalCount, int PageNumber, int PageSize, int TotalPages);