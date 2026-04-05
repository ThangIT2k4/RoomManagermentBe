using Auth.Application.Common;
using Auth.Application.Dtos;
using MediatR;

namespace Auth.Application.Features.Auth.Profile.UpdateProfile;

public sealed record UpdateProfileCommand(
    Guid UserId,
    string? FullName,
    DateOnly? Dob,
    short? Gender,
    string? Address,
    string? Note) : IRequest<Result<UserProfileDto>>;
