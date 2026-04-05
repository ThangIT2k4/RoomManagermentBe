using Auth.Application.Common;
using Auth.Application.Dtos;
using MediatR;

namespace Auth.Application.Features.Auth.Profile.UpdatePersonalInfo;

public sealed record UpdatePersonalInfoCommand(
    Guid UserId,
    string? FullName,
    DateOnly? Dob,
    short? Gender,
    string? Address,
    string? Note) : IRequest<Result<UserProfileDto>>;
