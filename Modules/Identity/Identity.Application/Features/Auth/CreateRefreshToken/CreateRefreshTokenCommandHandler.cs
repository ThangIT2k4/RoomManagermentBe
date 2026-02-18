using Identity.Application.Common;
using Identity.Domain.Entities;
using Identity.Domain.Repositories;
using Identity.Domain.ValueObjects;
using MediatR;

namespace Identity.Application.Features.Auth.CreateRefreshToken;

public sealed class CreateRefreshTokenCommandHandler(
    IUserRepository userRepository,
    IRefreshTokenRepository refreshTokenRepository,
    IUnitOfWork unitOfWork)
    : IRequestHandler<CreateRefreshTokenCommand, Result>
{
    public async Task<Result> Handle(CreateRefreshTokenCommand command, CancellationToken cancellationToken = default)
    {
        var user = await userRepository.GetByIdAsync(command.UserId, cancellationToken);
        if (user is null)
        {
            return Result.Failure(
                new Error("User.NotFound", $"User with id '{command.UserId}' was not found."));
        }

        var tokenValue = TokenValue.Create(command.Token);

        var refreshToken = RefreshTokenEntity.Create(
            Guid.NewGuid(),
            user.Id,
            tokenValue,
            command.ExpiresAt);

        await refreshTokenRepository.AddAsync(refreshToken, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}

