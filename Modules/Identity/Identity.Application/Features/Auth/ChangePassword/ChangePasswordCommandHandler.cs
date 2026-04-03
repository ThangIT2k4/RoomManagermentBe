using Identity.Application.Common;
using Identity.Domain.Repositories;
using Identity.Domain.ValueObjects;
using MassTransit;
using MediatR;
using Microsoft.Extensions.Logging;
using RoomManagerment.Messaging.Contracts.Events;

namespace Identity.Application.Features.Auth.ChangePassword;

public sealed class ChangePasswordCommandHandler(
    IUserRepository userRepository,
    IUnitOfWork unitOfWork,
    IPublishEndpoint publichEndpoint,
    ILogger<ChangePasswordCommandHandler> logger
    ) : IRequestHandler<ChangePasswordCommand, Result<bool>>
{
    public async Task<Result<bool>> Handle(ChangePasswordCommand request, CancellationToken cancellationToken)
    {
        var user = await userRepository.GetByIdAsync(request.UserId, cancellationToken);
        if (user is null)
        {
            logger.LogWarning("Attempt to change password for non-existent user: {UserId}", request.UserId);
            return Result<bool>.Failure(new Error("USER_NOT_FOUND", "User not found"));
        }
        
        var isCurrentPasswordValid = BCrypt.Net.BCrypt.Verify(request.CurrentPassword, user.PasswordHash.Value);
        if (!isCurrentPasswordValid)
            return Result<bool>.Failure(new Error("INVALID_CURRENT_PASSWORD", "Current password is incorrect"));
        

        var newHashedPassword = BCrypt.Net.BCrypt.HashPassword(request.NewPassword);
        user.ChangePassword(PasswordHash.Create(newHashedPassword));

        await userRepository.UpdateAsync(user, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        await publichEndpoint.Publish(new PasswordChangedEvent()
        {
            UserId = user.Id,
            ChangedAt = DateTime.UtcNow,
            SourceService = "Identity"
        }, cancellationToken);

        logger.LogInformation("Password changed successfully for user: {UserId}", request.UserId);
        return Result<bool>.Success(true);
    }
}