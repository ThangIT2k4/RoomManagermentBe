using Auth.Application.Common;
using Auth.Application.Dtos;
using RoomManagerment.Shared.Messaging;

namespace Auth.Application.Features.Auth.Profile.UploadAvatar;

public sealed record UploadAvatarCommand(Guid UserId, string AvatarUrl) : IAppRequest<Result<UserProfileDto>>;
