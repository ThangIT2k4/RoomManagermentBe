using Auth.Application.Common;
using Auth.Application.Dtos;
using RoomManagerment.Shared.Messaging;

namespace Auth.Application.Features.Auth.Profile.UpdatePersonalInfo;

public sealed record UpdatePersonalInfoCommand(
    Guid UserId,
    string? FullName,
    DateOnly? Dob,
    short? Gender,
    string? Address,
    string? Note) : IAppRequest<Result<UserProfileDto>>;
