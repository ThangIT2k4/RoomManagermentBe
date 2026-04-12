using Auth.Application.Common;
using Auth.Application.Dtos;
using Auth.Application.Services;
using RoomManagerment.Shared.Messaging;

namespace Auth.Application.Features.Auth.Profile.UploadAvatar;

public sealed class UploadAvatarCommandHandler(IAuthApplicationService authService)
    : IAppRequestHandler<UploadAvatarCommand, Result<UserProfileDto>>
{
    public Task<Result<UserProfileDto>> Handle(UploadAvatarCommand request, CancellationToken cancellationToken)
        => authService.UploadAvatarAsync(new UploadAvatarRequest(request.UserId, request.AvatarUrl), cancellationToken);
}
