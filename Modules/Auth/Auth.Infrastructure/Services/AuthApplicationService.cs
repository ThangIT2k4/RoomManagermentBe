using Auth.Application.Common;
using Auth.Application.Dtos;
using Auth.Application.Services;
using Auth.Domain.Common;
using Auth.Domain.Entities;
using Auth.Domain.Enums;
using Auth.Domain.Repositories;
using Auth.Domain.ValueObjects;
using Auth.Infrastructure.Mapper;
using RoomManagerment.Auth.DatabaseSpecific;
using RoomManagerment.Auth.Linq;
using SD.LLBLGen.Pro.LinqSupportClasses;
using AuthDataAccessAdapter = RoomManagerment.Auth.DatabaseSpecific.DataAccessAdapter;
using AuthLinqMetaData = RoomManagerment.Auth.Linq.LinqMetaData;
using DalEmailOtpEntity = RoomManagerment.Auth.EntityClasses.EmailOtpEntity;
using DalUserProfileEntity = RoomManagerment.Auth.EntityClasses.UserProfileEntity;

namespace Auth.Infrastructure.Services;

public sealed class AuthApplicationService(
    IUserRepository userRepository,
    ISessionRepository sessionRepository,
    IPasswordHasher passwordHasher,
    IOrganizationMembershipGateway organizationGateway,
    AuthDataAccessAdapter adapter) : IAuthApplicationService
{
    private static TimeSpan OtpExpiryFor(OtpPurpose purpose) => purpose switch
    {
        OtpPurpose.ResetPassword => TimeSpan.FromMinutes(15),
        _ => TimeSpan.FromMinutes(5)
    };

    public async Task<Result<RegisterResult>> RegisterAsync(RegisterRequest request, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(request.Password))
        {
            return Result<RegisterResult>.Failure(new Error("Auth.Password.Empty", "Password is required."));
        }

        var email = Email.Create(request.Email);
        if (await userRepository.ExistsByEmailAsync(email, cancellationToken: cancellationToken))
        {
            return Result<RegisterResult>.Failure(new Error("Auth.Email.Exists", "Email already exists."));
        }

        Username? username = null;
        if (!string.IsNullOrWhiteSpace(request.Username))
        {
            username = Username.Create(request.Username);
            if (await userRepository.ExistsByUsernameAsync(username, cancellationToken: cancellationToken))
            {
                return Result<RegisterResult>.Failure(new Error("Auth.Username.Exists", "Username already exists."));
            }
        }

        var user = UserEntity.Create(
            email,
            username,
            string.IsNullOrWhiteSpace(request.Phone) ? null : Phone.Create(request.Phone),
            PasswordHash.Create(passwordHasher.Hash(request.Password)),
            (short)UserStatus.Inactive,
            DateTime.UtcNow);

        await userRepository.AddAsync(user, cancellationToken);

        var now = DateTime.UtcNow;
        var profile = new DalUserProfileEntity
        {
            UserId = user.Id,
            FullName = request.FullName.Trim(),
            CreatedAt = now,
            UpdatedAt = now
        };
        await adapter.SaveEntityAsync(profile, true, false, cancellationToken);

        var otpResult = await SendOtpAsync(
            new SendOtpRequest(user.Email.Value, OtpPurpose.VerifyEmail, user.Id),
            cancellationToken);
        if (otpResult.IsFailure)
        {
            return Result<RegisterResult>.Failure(otpResult.Error ?? new Error("Auth.Otp.Failed", "Could not issue verification OTP."));
        }

        return Result<RegisterResult>.Success(new RegisterResult(MapUser(user)));
    }

    public async Task<Result<LoginResult>> LoginAsync(LoginRequest request, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(request.Login) || string.IsNullOrWhiteSpace(request.Password))
        {
            return Result<LoginResult>.Failure(new Error("Auth.Login.Invalid", "Login or password is invalid."));
        }

        UserEntity? user;
        if (request.Login.Contains('@'))
        {
            user = await userRepository.GetByEmailAsync(Email.Create(request.Login), cancellationToken);
        }
        else
        {
            user = await userRepository.GetByUsernameAsync(Username.Create(request.Login), cancellationToken);
        }

        if (user is null || user.PasswordHash is null)
        {
            return Result<LoginResult>.Failure(new Error("Auth.Login.Failed", "Invalid credentials."));
        }

        if (user.Status == UserStatus.Inactive)
        {
            return Result<LoginResult>.Failure(new Error("Auth.Login.NotVerified", "Account is not verified. Please verify your email."));
        }

        if (user.Status == UserStatus.Banned)
        {
            return Result<LoginResult>.Failure(new Error("Auth.Login.Banned", "Account is locked."));
        }

        if (!passwordHasher.Verify(request.Password, user.PasswordHash.Value))
        {
            return Result<LoginResult>.Failure(new Error("Auth.Login.Failed", "Invalid credentials."));
        }

        user.RecordLogin(DateTime.UtcNow, null, request.IpAddress);
        await userRepository.UpdateAsync(user, cancellationToken);

        var sessionResult = await CreateSessionAsync(new CreateSessionRequest(user.Id, request.IpAddress, request.UserAgent, request.RememberMe), cancellationToken);
        if (sessionResult.IsFailure || sessionResult.Value is null)
        {
            return Result<LoginResult>.Failure(sessionResult.Error ?? new Error("Auth.Session.Failed", "Cannot create session."));
        }

        return Result<LoginResult>.Success(new LoginResult(MapUser(user), sessionResult.Value));
    }

    public async Task<Result> LogoutAsync(LogoutRequest request, CancellationToken cancellationToken = default)
    {
        await sessionRepository.DeleteAsync(request.SessionId, cancellationToken);
        return Result.Success();
    }

    public async Task<Result<UserDto>> GetCurrentUserAsync(GetCurrentUserRequest request, CancellationToken cancellationToken = default)
    {
        var session = await sessionRepository.GetByIdAsync(request.SessionId, cancellationToken);
        if (session is null || session.IsExpired(DateTimeOffset.UtcNow))
        {
            return Result<UserDto>.Failure(new Error("Auth.Session.NotFound", "Session not found or expired."));
        }

        var user = await userRepository.GetByIdAsync(session.UserId, cancellationToken);
        if (user is null)
        {
            return Result<UserDto>.Failure(new Error("Auth.User.NotFound", "User not found."));
        }

        return Result<UserDto>.Success(MapUser(user));
    }

    public async Task<Result<UserDto>> GetUserByIdAsync(GetUserByIdRequest request, CancellationToken cancellationToken = default)
    {
        var user = await userRepository.GetByIdAsync(request.UserId, cancellationToken);
        if (user is null)
        {
            return Result<UserDto>.Failure(new Error("Auth.User.NotFound", "User not found."));
        }

        return Result<UserDto>.Success(MapUser(user));
    }

    public async Task<Result<PagedUsersResult>> GetUsersAsync(GetUsersRequest request, CancellationToken cancellationToken = default)
    {
        var paged = await userRepository.SearchPagedAsync(request.SearchTerm, request.PageNumber, request.PageSize, request.IncludeDeleted, cancellationToken);
        return Result<PagedUsersResult>.Success(new PagedUsersResult(
            paged.Items.Select(MapUser).ToList(),
            paged.TotalCount,
            paged.Page,
            paged.PageSize,
            paged.TotalPages));
    }

    public async Task<Result<UserDto>> UpdateUserAsync(UpdateUserRequest request, CancellationToken cancellationToken = default)
    {
        var user = await userRepository.GetByIdAsync(request.UserId, cancellationToken);
        if (user is null)
        {
            return Result<UserDto>.Failure(new Error("Auth.User.NotFound", "User not found."));
        }

        if (!string.IsNullOrWhiteSpace(request.Email))
        {
            var email = Email.Create(request.Email);
            if (await userRepository.ExistsByEmailAsync(email, request.UserId, cancellationToken))
            {
                return Result<UserDto>.Failure(new Error("Auth.Email.Exists", "Email already exists."));
            }
            user.ChangeEmail(email, DateTime.UtcNow);
        }

        if (!string.IsNullOrWhiteSpace(request.Username))
        {
            var username = Username.Create(request.Username);
            if (await userRepository.ExistsByUsernameAsync(username, request.UserId, cancellationToken))
            {
                return Result<UserDto>.Failure(new Error("Auth.Username.Exists", "Username already exists."));
            }
            user.ChangeUsername(username, DateTime.UtcNow);
        }

        if (request.Phone is not null)
        {
            user.ChangePhone(string.IsNullOrWhiteSpace(request.Phone) ? null : Phone.Create(request.Phone), DateTime.UtcNow);
        }

        if (request.Status.HasValue)
        {
            ApplyStatus(user, request.Status.Value, DateTime.UtcNow);
        }

        await userRepository.UpdateAsync(user, cancellationToken);
        return Result<UserDto>.Success(MapUser(user));
    }

    public async Task<Result> DeleteUserAsync(DeleteUserRequest request, CancellationToken cancellationToken = default)
    {
        await userRepository.SoftDeleteAsync(request.UserId, request.DeletedBy, DateTime.UtcNow, cancellationToken);
        return Result.Success();
    }

    public async Task<Result> ChangePasswordAsync(ChangePasswordRequest request, CancellationToken cancellationToken = default)
    {
        var user = await userRepository.GetByIdAsync(request.UserId, cancellationToken);
        if (user is null || user.PasswordHash is null)
        {
            return Result.Failure(new Error("Auth.User.NotFound", "User not found."));
        }

        if (!passwordHasher.Verify(request.CurrentPassword, user.PasswordHash.Value))
        {
            return Result.Failure(new Error("Auth.Password.Invalid", "Current password is invalid."));
        }

        user.SetPassword(PasswordHash.Create(passwordHasher.Hash(request.NewPassword)), DateTime.UtcNow);
        await userRepository.UpdateAsync(user, cancellationToken);

        if (!string.IsNullOrWhiteSpace(request.RetainSessionId))
        {
            await sessionRepository.DeleteAllByUserExceptAsync(request.UserId, request.RetainSessionId, cancellationToken);
        }

        return Result.Success();
    }

    public async Task<Result> ForgotPasswordAsync(ForgotPasswordRequest request, CancellationToken cancellationToken = default)
    {
        return await SendOtpAsync(new SendOtpRequest(request.Email, OtpPurpose.ResetPassword), cancellationToken);
    }

    public async Task<Result> ResetPasswordAsync(ResetPasswordRequest request, CancellationToken cancellationToken = default)
    {
        var otp = await VerifyOtpCoreAsync(request.Email, OtpPurpose.ResetPassword, request.OtpCode, cancellationToken);
        if (otp is null)
        {
            return Result.Failure(new Error("Auth.Otp.Invalid", "OTP is invalid or expired."));
        }

        var user = await userRepository.GetByEmailAsync(Email.Create(request.Email), cancellationToken);
        if (user is null)
        {
            return Result.Failure(new Error("Auth.User.NotFound", "User not found."));
        }

        user.SetPassword(PasswordHash.Create(passwordHasher.Hash(request.NewPassword)), DateTime.UtcNow);
        user.ClearRememberToken(DateTime.UtcNow);
        await userRepository.UpdateAsync(user, cancellationToken);
        await sessionRepository.DeleteAllByUserAsync(user.Id, cancellationToken);
        return Result.Success();
    }

    public async Task<Result> SendOtpAsync(SendOtpRequest request, CancellationToken cancellationToken = default)
    {
        var linq = new AuthLinqMetaData(adapter);
        var now = DateTime.UtcNow;
        var otp = GenerateOtp();

        var entity = new DalEmailOtpEntity
        {
            Id = Guid.NewGuid(),
            UserId = request.UserId,
            Email = request.Email.Trim().ToLowerInvariant(),
            OtpCode = otp,
            Type = MapOtpPurpose(request.Purpose),
            ExpiresAt = now.Add(OtpExpiryFor(request.Purpose)),
            IsUsed = false,
            CreatedAt = now,
            UpdatedAt = now
        };

        await adapter.SaveEntityAsync(entity, true, false, cancellationToken);
        return Result.Success();
    }

    public async Task<Result> VerifyOtpAsync(VerifyOtpRequest request, CancellationToken cancellationToken = default)
    {
        var otp = await VerifyOtpCoreAsync(request.Email, request.Purpose, request.OtpCode, cancellationToken);
        return otp is null
            ? Result.Failure(new Error("Auth.Otp.Invalid", "OTP is invalid or expired."))
            : Result.Success();
    }

    public async Task<Result> VerifyEmailAsync(VerifyEmailRequest request, CancellationToken cancellationToken = default)
    {
        var otp = await VerifyOtpCoreAsync(request.Email, OtpPurpose.VerifyEmail, request.OtpCode, cancellationToken);
        if (otp is null)
        {
            return Result.Failure(new Error("Auth.Otp.Invalid", "OTP is invalid or expired."));
        }

        if (!otp.UserId.HasValue || otp.UserId.Value == Guid.Empty)
        {
            return Result.Failure(new Error("Auth.Otp.Invalid", "OTP is not bound to a user."));
        }

        var user = await userRepository.GetByIdAsync(otp.UserId.Value, cancellationToken);
        if (user is null)
        {
            return Result.Failure(new Error("Auth.User.NotFound", "User not found."));
        }

        if (!string.Equals(user.Email.Value, request.Email.Trim(), StringComparison.OrdinalIgnoreCase))
        {
            return Result.Failure(new Error("Auth.Otp.Invalid", "OTP does not match this email."));
        }

        user.VerifyEmailAndActivate(DateTime.UtcNow);
        await userRepository.UpdateAsync(user, cancellationToken);
        return Result.Success();
    }

    public async Task<Result> ResendOtpAsync(ResendOtpRequest request, CancellationToken cancellationToken = default)
    {
        return await SendOtpAsync(new SendOtpRequest(request.Email, request.Purpose, request.UserId), cancellationToken);
    }

    public async Task<Result<SessionDto>> CreateSessionAsync(CreateSessionRequest request, CancellationToken cancellationToken = default)
    {
        var token = Convert.ToHexString(Guid.NewGuid().ToByteArray()) + Convert.ToHexString(Guid.NewGuid().ToByteArray());
        if (token.Length > 128)
        {
            token = token[..128];
        }

        var expiresAt = request.RememberMe ? DateTimeOffset.UtcNow.AddDays(30) : (DateTimeOffset?)null;
        var session = SessionEntity.Create(request.UserId, token, request.IpAddress, request.UserAgent, null, expiresAt, DateTime.UtcNow);
        await sessionRepository.AddAsync(session, cancellationToken);
        return Result<SessionDto>.Success(MapSession(session));
    }

    public async Task<Result<PagedSessionsResult>> GetActiveSessionsAsync(GetActiveSessionsRequest request, CancellationToken cancellationToken = default)
    {
        var paged = await sessionRepository.GetByUserPagedAsync(request.UserId, request.PageNumber, request.PageSize, false, cancellationToken);
        return Result<PagedSessionsResult>.Success(new PagedSessionsResult(
            paged.Items.Select(MapSession).ToList(),
            paged.TotalCount,
            paged.Page,
            paged.PageSize,
            paged.TotalPages));
    }

    public async Task<Result> LogoutAllSessionsAsync(LogoutAllSessionsRequest request, CancellationToken cancellationToken = default)
    {
        await sessionRepository.DeleteAllByUserAsync(request.UserId, cancellationToken);
        return Result.Success();
    }

    public async Task<Result<UserDto>> ChangeUserStatusAsync(ChangeUserStatusRequest request, CancellationToken cancellationToken = default)
    {
        var user = await userRepository.GetByIdAsync(request.UserId, cancellationToken);
        if (user is null)
        {
            return Result<UserDto>.Failure(new Error("Auth.User.NotFound", "User not found."));
        }

        ApplyStatus(user, request.Status, DateTime.UtcNow);
        await userRepository.UpdateAsync(user, cancellationToken);
        return Result<UserDto>.Success(MapUser(user));
    }

    public async Task<Result<UserProfileDto>> GetProfileAsync(GetProfileRequest request, CancellationToken cancellationToken = default)
    {
        var linq = new AuthLinqMetaData(adapter);
        var profile = await linq.UserProfile.Where(x => x.UserId == request.UserId).FirstOrDefaultAsync(cancellationToken);
        if (profile is null)
        {
            return Result<UserProfileDto>.Failure(new Error("Auth.Profile.NotFound", "Profile not found."));
        }

        return Result<UserProfileDto>.Success(MapProfile(profile));
    }

    public async Task<Result<UserProfileDto>> UpdateProfileAsync(UpdateProfileRequest request, CancellationToken cancellationToken = default)
    {
        var profile = await EnsureProfileAsync(request.UserId, cancellationToken);
        profile.FullName = request.FullName?.Trim();
        profile.Dob = request.Dob ?? profile.Dob;
        profile.Gender = request.Gender;
        profile.Address = request.Address?.Trim();
        profile.Note = request.Note?.Trim();
        profile.UpdatedAt = DateTime.UtcNow;

        await adapter.SaveEntityAsync(profile, true, false, cancellationToken);
        return Result<UserProfileDto>.Success(MapProfile(profile));
    }

    public async Task<Result<UserProfileDto>> UploadAvatarAsync(UploadAvatarRequest request, CancellationToken cancellationToken = default)
    {
        var profile = await EnsureProfileAsync(request.UserId, cancellationToken);
        profile.Avatar = request.AvatarUrl?.Trim();
        profile.UpdatedAt = DateTime.UtcNow;

        await adapter.SaveEntityAsync(profile, true, false, cancellationToken);
        return Result<UserProfileDto>.Success(MapProfile(profile));
    }

    public async Task<Result<UserProfileDto>> UpdatePersonalInfoAsync(UpdatePersonalInfoRequest request, CancellationToken cancellationToken = default)
    {
        return await UpdateProfileAsync(new UpdateProfileRequest(request.UserId, request.FullName, request.Dob, request.Gender, request.Address, request.Note), cancellationToken);
    }

    public Task<Result> AssignRoleAsync(AssignRoleRequest request, CancellationToken cancellationToken = default)
        => organizationGateway.AssignRoleAsync(request, cancellationToken);

    public Task<Result> RemoveRoleAsync(RemoveRoleRequest request, CancellationToken cancellationToken = default)
        => organizationGateway.RemoveRoleAsync(request, cancellationToken);

    public Task<Result<IReadOnlyList<RoleDto>>> GetUserRolesAsync(GetUserRolesRequest request, CancellationToken cancellationToken = default)
        => organizationGateway.GetUserRolesAsync(request, cancellationToken);

    public async Task<Result<PagedCapabilitiesResult>> GetCapabilitiesAsync(GetCapabilitiesRequest request, CancellationToken cancellationToken = default)
    {
        var paging = PagingInput.Create(request.PageNumber, request.PageSize);
        var linq = new AuthLinqMetaData(adapter);
        var query = linq.Capability.AsQueryable();

        var search = SearchInput.Normalize(request.SearchTerm);
        if (!string.IsNullOrWhiteSpace(search))
        {
            query = query.Where(x => x.KeyCode.Contains(search) || x.Name.Contains(search) || (x.Description != null && x.Description.Contains(search)));
        }

        var total = await query.LongCountAsync(cancellationToken);
        var items = await query
            .OrderBy(x => x.DisplayOrder)
            .ThenBy(x => x.Name)
            .Skip(paging.Skip)
            .Take(paging.PageSize)
            .ToListAsync(cancellationToken);

        var mapped = items.Select(x => new CapabilityDto(x.Id, x.KeyCode, x.Name, x.Description, x.Category, x.DisplayOrder)).ToList();
        return Result<PagedCapabilitiesResult>.Success(new PagedCapabilitiesResult(mapped, total, paging.PageNumber, paging.PageSize, total == 0 ? 0 : (int)Math.Ceiling(total / (double)paging.PageSize)));
    }

    private async Task<DalUserProfileEntity> EnsureProfileAsync(Guid userId, CancellationToken cancellationToken)
    {
        var linq = new AuthLinqMetaData(adapter);
        var profile = await linq.UserProfile.Where(x => x.UserId == userId).FirstOrDefaultAsync(cancellationToken);
        if (profile is not null)
        {
            return profile;
        }

        profile = new DalUserProfileEntity
        {
            UserId = userId,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };
        await adapter.SaveEntityAsync(profile, true, false, cancellationToken);
        return profile;
    }

    private async Task<DalEmailOtpEntity?> VerifyOtpCoreAsync(string email, OtpPurpose purpose, string otpCode, CancellationToken cancellationToken)
    {
        var linq = new AuthLinqMetaData(adapter);
        var now = DateTime.UtcNow;

        var otp = await linq.EmailOtp
            .Where(x => x.Email == email.Trim().ToLowerInvariant()
                        && x.Type == MapOtpPurpose(purpose)
                        && x.OtpCode == otpCode
                        && x.IsUsed == false
                        && x.ExpiresAt > now)
            .OrderByDescending(x => x.CreatedAt)
            .FirstOrDefaultAsync(cancellationToken);

        if (otp is null)
        {
            return null;
        }

        otp.IsUsed = true;
        otp.VerifiedAt = now;
        otp.UpdatedAt = now;
        await adapter.SaveEntityAsync(otp, true, false, cancellationToken);
        return otp;
    }

    private static void ApplyStatus(UserEntity user, short status, DateTime changedAt)
    {
        switch ((UserStatus)status)
        {
            case UserStatus.Active:
                user.Activate(changedAt);
                break;
            case UserStatus.Inactive:
                user.Deactivate(changedAt);
                break;
            case UserStatus.Banned:
                user.Ban(changedAt);
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(status), "Invalid user status.");
        }
    }

    private static string GenerateOtp()
    {
        return Random.Shared.Next(100000, 999999).ToString();
    }

    private static string MapOtpPurpose(OtpPurpose purpose)
    {
        return purpose switch
        {
            OtpPurpose.VerifyEmail => "verify_email",
            OtpPurpose.ResetPassword => "reset_password",
            _ => "verify_email"
        };
    }

    private static UserDto MapUser(UserEntity user)
    {
        return new UserDto(
            user.Id,
            user.Email.Value,
            user.Username?.Value,
            (short)user.Status,
            user.CreatedAt,
            user.UpdatedAt,
            user.EmailVerifiedAt,
            user.LastLoginAt,
            user.Phone?.Value);
    }

    private static SessionDto MapSession(SessionEntity session)
    {
        return new SessionDto(
            session.Id,
            session.UserId,
            session.IpAddress,
            session.UserAgent,
            session.LastActivity.UtcDateTime,
            session.ExpiresAt?.UtcDateTime,
            session.CreatedAt);
    }

    private static UserProfileDto MapProfile(DalUserProfileEntity profile)
    {
        return new UserProfileDto(
            profile.UserId,
            profile.FullName,
            profile.Avatar,
            profile.Dob,
            profile.Gender,
            profile.Address,
            profile.Note,
            profile.CreatedAt,
            profile.UpdatedAt);
    }
}


