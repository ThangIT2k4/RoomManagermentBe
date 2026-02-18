using Identity.Application.Common;
using Identity.Domain.Repositories;
using MediatR;

namespace Identity.Application.Features.Users.ActivateUser;

public sealed class ActivateUserCommandHandler(IUserRepository userRepository, IUnitOfWork unitOfWork)
    : IRequestHandler<ActivateUserCommand, Result>
{
    public async Task<Result> Handle(ActivateUserCommand command, CancellationToken cancellationToken = default)
    {
        var user = await userRepository.GetByIdAsync(command.UserId, cancellationToken);

        if (user is null)
        {
            return Result.Failure(
                new Error("User.NotFound", $"User with id '{command.UserId}' was not found."));
        }

        user.Activate(DateTime.UtcNow);

        await userRepository.UpdateAsync(user, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}

