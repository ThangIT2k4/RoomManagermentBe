using Auth.Application.Common;
using Auth.Application.Dtos;
using MediatR;

namespace Auth.Application.Features.Auth.Profile.UploadAvatar;

public sealed record UploadAvatarCommand(Guid UserId, string AvatarUrl) : IRequest<Result<UserProfileDto>>;
