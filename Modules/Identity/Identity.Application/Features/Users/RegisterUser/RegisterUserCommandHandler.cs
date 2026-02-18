using Identity.Application.Common;
using Identity.Domain.Entities;
using Identity.Domain.Repositories;
using Identity.Domain.ValueObjects;
using MediatR;

namespace Identity.Application.Features.Users.RegisterUser;

public sealed class RegisterUserCommandHandler(IUserRepository userRepository, IUnitOfWork unitOfWork)
    : IRequestHandler<RegisterUserCommand, Result<RegisterUserResult>>
{
    public async Task<Result<RegisterUserResult>> Handle(RegisterUserCommand command, CancellationToken cancellationToken = default)
    {
        if (await userRepository.ExistsByUsernameAsync(command.Username, cancellationToken))
        {
            return Result<RegisterUserResult>.Failure(
                new Error("User.DuplicateUsername", "Username already exists."));
        }

        if (await userRepository.ExistsByEmailAsync(command.Email, cancellationToken))
        {
            return Result<RegisterUserResult>.Failure(
                new Error("User.DuplicateEmail", "Email already exists."));
        }
        
        var username = Username.Create(command.Username);
        var email = Email.Create(command.Email);
        // Ở đây tạm thời giả sử password đã được hash ở ngoài và truyền vào,
        // hoặc bạn sẽ thay thế bằng IPasswordHasher ở Application/Infrastructure.
        var passwordHash = PasswordHash.Create(command.Password);

        var user = UserEntity.Create(Guid.NewGuid(), username, email, passwordHash);

        user = await userRepository.AddAsync(user, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        var result = new RegisterUserResult(
            user.Id,
            user.Username.Value,
            user.Email.Value,
            user.Status.ToString());

        return Result<RegisterUserResult>.Success(result);
    }
}

