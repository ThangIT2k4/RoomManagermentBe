using Auth.Application.Common;
using Auth.Application.Dtos;
using MediatR;

namespace Auth.Application.Features.Auth.Profile.GetProfile;

public sealed record GetProfileQuery(Guid UserId) : IRequest<Result<UserProfileDto>>;
