using Auth.Application.Common;
using Auth.Application.Dtos;
using Auth.Application.Features.Register;

namespace Auth.Application.Services;

public interface IAuthApplicationService
{
    Task<Result<RegisterResult>> RegisterAsync(RegisterRequest request, CancellationToken cancellationToken = default);
    Task<Result<LoginResult>> LoginAsync(LoginRequest request, CancellationToken cancellationToken = default);
    Task<Result> LogoutAsync(LogoutRequest request, CancellationToken cancellationToken = default);
    Task<Result<UserDto>> GetCurrentUserAsync(GetCurrentUserRequest request, CancellationToken cancellationToken = default);
    Task<Result<UserDto>> GetUserByIdAsync(GetUserByIdRequest request, CancellationToken cancellationToken = default);
    Task<Result<PagedUsersResult>> GetUsersAsync(GetUsersRequest request, CancellationToken cancellationToken = default);
    Task<Result<UserDto>> UpdateUserAsync(UpdateUserRequest request, CancellationToken cancellationToken = default);
    Task<Result> DeleteUserAsync(DeleteUserRequest request, CancellationToken cancellationToken = default);

    Task<Result> ChangePasswordAsync(ChangePasswordRequest request, CancellationToken cancellationToken = default);
    Task<Result> ForgotPasswordAsync(ForgotPasswordRequest request, CancellationToken cancellationToken = default);
    Task<Result> ResetPasswordAsync(ResetPasswordRequest request, CancellationToken cancellationToken = default);

    Task<Result> SendOtpAsync(SendOtpRequest request, CancellationToken cancellationToken = default);
    Task<Result> VerifyOtpAsync(VerifyOtpRequest request, CancellationToken cancellationToken = default);
    Task<Result> VerifyEmailAsync(VerifyEmailRequest request, CancellationToken cancellationToken = default);
    Task<Result> ResendOtpAsync(ResendOtpRequest request, CancellationToken cancellationToken = default);

    Task<Result<SessionDto>> CreateSessionAsync(CreateSessionRequest request, CancellationToken cancellationToken = default);
    Task<Result<PagedSessionsResult>> GetActiveSessionsAsync(GetActiveSessionsRequest request, CancellationToken cancellationToken = default);
    Task<Result> LogoutAllSessionsAsync(LogoutAllSessionsRequest request, CancellationToken cancellationToken = default);

    Task<Result<UserDto>> ChangeUserStatusAsync(ChangeUserStatusRequest request, CancellationToken cancellationToken = default);

    Task<Result<UserProfileDto>> GetProfileAsync(GetProfileRequest request, CancellationToken cancellationToken = default);
    Task<Result<UserProfileDto>> UpdateProfileAsync(UpdateProfileRequest request, CancellationToken cancellationToken = default);
    Task<Result<UserProfileDto>> UploadAvatarAsync(UploadAvatarRequest request, CancellationToken cancellationToken = default);
    Task<Result<UserProfileDto>> UpdatePersonalInfoAsync(UpdatePersonalInfoRequest request, CancellationToken cancellationToken = default);

    Task<Result> AssignRoleAsync(AssignRoleRequest request, CancellationToken cancellationToken = default);
    Task<Result> RemoveRoleAsync(RemoveRoleRequest request, CancellationToken cancellationToken = default);
    Task<Result<IReadOnlyList<RoleDto>>> GetUserRolesAsync(GetUserRolesRequest request, CancellationToken cancellationToken = default);
    Task<Result<PagedCapabilitiesResult>> GetCapabilitiesAsync(GetCapabilitiesRequest request, CancellationToken cancellationToken = default);
}

