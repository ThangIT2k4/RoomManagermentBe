using Auth.Application.Common;
using Auth.Application.Dtos;
using RoomManagerment.Shared.Messaging;

namespace Auth.Application.Features.Auth.Profile.UpdateProfile;

public sealed record UpdateProfileCommand(
    Guid UserId,
    string? FullName,
    DateOnly? Dob,
    short? Gender,
    string? Address,
    string? Note) : IAppRequest<Result<UserProfileDto>>;
