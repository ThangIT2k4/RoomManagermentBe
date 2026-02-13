using Identity.Application.Common;
using Identity.Domain.Repositories;

namespace Identity.Application.Features.Users.GetUserById;

public sealed class GetUserByIdQueryHandler(IUserRepository userRepository)
{
    public async Task<Result<UserDto>> HandleAsync(GetUserByIdQuery query, CancellationToken cancellationToken = default)
    {
        var user = await userRepository.GetByIdAsync(query.UserId, cancellationToken);

        if (user is null)
        {
            return Result<UserDto>.Failure(
                new Error("User.NotFound", $"User with id '{query.UserId}' was not found."));
        }

        var dto = new UserDto(
            user.Id,
            user.Username.Value,
            user.Email.Value,
            user.Status.ToString());

        return Result<UserDto>.Success(dto);
    }
}

