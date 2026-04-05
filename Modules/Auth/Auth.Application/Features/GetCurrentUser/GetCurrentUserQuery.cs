using Auth.Application.Common;
using Auth.Application.Dtos;
using MediatR;

namespace Auth.Application.Features.GetCurrentUser;

public sealed record GetCurrentUserQuery(string SessionId) : IRequest<Result<UserDto>>;
