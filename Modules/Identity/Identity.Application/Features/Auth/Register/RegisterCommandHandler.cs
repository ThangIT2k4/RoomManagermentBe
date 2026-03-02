using Identity.Application.Common;
using Identity.Domain.Entities;
using Identity.Domain.Repositories;
using Identity.Domain.ValueObjects;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Identity.Application.Features.Auth.Register;

public class RegisterCommandHandler(
    IUserRepository userRepository,
    IUnitOfWork unitOfWork,
    ILogger<RegisterCommandHandler> logger) 
    : IRequestHandler<RegisterCommand, Result<RegisterResult>>
{
    public async Task<Result<RegisterResult>> Handle(RegisterCommand request, CancellationToken cancellationToken)
    {
        try
        {
            // Check if user already exists with email
            var existingUserByEmail = await userRepository.GetByEmailAsync(
                request.Email, 
                cancellationToken);
            
            if (existingUserByEmail != null)
            {
                var error = new Error("EMAIL_EXISTS", "Email đã được đăng ký");
                return Result<RegisterResult>.Failure(error);
            }

            // Check if username exists
            var existingUserByUsername = await userRepository.GetByUsernameAsync(
                request.Username, 
                cancellationToken);
            
            if (existingUserByUsername != null)
            {
                var error = new Error("USERNAME_EXISTS", "Tên đăng nhập đã được sử dụng");
                return Result<RegisterResult>.Failure(error);
            }

            // Hash password
            var hashedPassword = BCrypt.Net.BCrypt.HashPassword(request.Password);
            
            // Create new user
            var passwordHash = PasswordHash.Create(hashedPassword);
            var newUser = UserEntity.Create(
                Guid.NewGuid(),
                Username.Create(request.Username),
                Email.Create(request.Email),
                passwordHash);

            // Add user to repository
            await userRepository.AddAsync(newUser, cancellationToken);
            await unitOfWork.SaveChangesAsync(cancellationToken);

            logger.LogInformation("User registered successfully: {Email}", request.Email);

            return Result<RegisterResult>.Success(new RegisterResult
            {
                UserId = newUser.Id,
                Username = newUser.Username.Value,
                Email = newUser.Email.Value
            });
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error registering user: {Email}", request.Email);
            var error = new Error("REGISTER_ERROR", "Có lỗi xảy ra trong quá trình đăng ký");
            return Result<RegisterResult>.Failure(error);
        }
    }
}
