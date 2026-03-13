using Identity.Application.Common;
using Identity.Domain.Enums;
using Identity.Domain.Repositories;
using MassTransit;
using MediatR;
using Microsoft.Extensions.Logging;
using RoomManagerment.Messaging.Contracts.Events;

namespace Identity.Application.Features.Auth.Login;

public class LoginCommandHandler(
    IUserRepository userRepository,
    IPublishEndpoint publishEndpoint,
    ILogger<LoginCommandHandler> logger) 
    : IRequestHandler<LoginCommand, Result<LoginResult>>
{
    public async Task<Result<LoginResult>> Handle(LoginCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var user = await userRepository.GetByEmailAsync(request.UsernameOrEmail, cancellationToken);
            
            if (user == null)
            {
                user = await userRepository.GetByUsernameAsync(request.UsernameOrEmail, cancellationToken);
            }

            if (user == null)
            {
                logger.LogWarning("Login attempt with non-existent user: {UsernameOrEmail}", request.UsernameOrEmail);
                var error = new Error("INVALID_CREDENTIALS", "Email hoặc tên đăng nhập không chính xác");
                return Result<LoginResult>.Failure(error);
            }

            // Verify password
            if (!BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash.Value))
            {
                logger.LogWarning("Failed login attempt for user: {Email}", user.Email.Value);
                var error = new Error("INVALID_CREDENTIALS", "Email hoặc mật khẩu không chính xác");
                return Result<LoginResult>.Failure(error);
            }

            // Check if user is active
            if (user.Status != UserStatus.Active)
            {
                logger.LogWarning("Login attempt by inactive user: {Email}", user.Email.Value);
                var error = new Error("USER_INACTIVE", "Tài khoản chưa được kích hoạt");
                return Result<LoginResult>.Failure(error);
            }

            logger.LogInformation("User logged in successfully: {Email}", user.Email.Value);

            // Publish event cho các service khác
            await publishEndpoint.Publish(new UserLoggedInEvent
            {
                UserId = user.Id,
                Username = user.Username.Value,
                IpAddress = request.IpAddress ?? "unknown",
                LoggedInAt = DateTime.UtcNow
            }, cancellationToken);

            // Generate access token (optional, can be empty if using session)
            var accessToken = string.Empty;
            var expiration = DateTime.UtcNow.AddMinutes(30);

            return Result<LoginResult>.Success(new LoginResult
            {
                UserId = user.Id,
                Username = user.Username.Value,
                Email = user.Email.Value,
                AccessToken = accessToken,
                Expiration = expiration
            });
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error during login for user: {UsernameOrEmail}", request.UsernameOrEmail);
            var error = new Error("LOGIN_ERROR", "Có lỗi xảy ra trong quá trình đăng nhập");
            return Result<LoginResult>.Failure(error);
        }
    }
}
