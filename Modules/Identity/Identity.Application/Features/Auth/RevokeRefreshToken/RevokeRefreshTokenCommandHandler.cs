using Identity.Application.Common;
using Identity.Domain.Repositories;
using MediatR;

namespace Identity.Application.Features.Auth.RevokeRefreshToken;

public sealed class RevokeRefreshTokenCommandHandler(
    IRefreshTokenRepository refreshTokenRepository,
    IUnitOfWork unitOfWork)
    : IRequestHandler<RevokeRefreshTokenCommand, Result>
{
    public async Task<Result> Handle(RevokeRefreshTokenCommand command, CancellationToken cancellationToken = default)
    {
        var refreshToken = await refreshTokenRepository.GetByTokenAsync(command.Token, cancellationToken);
        if (refreshToken is null)
        {
            return Result.Failure(
                new Error("RefreshToken.NotFound", "Refresh token was not found."));
        }

        if (!refreshToken.IsValid)
        {
            return Result.Failure(
                new Error("RefreshToken.Invalid", "Refresh token is invalid or expired."));
        }

        refreshToken.Revoke();

        await refreshTokenRepository.UpdateAsync(refreshToken, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}

