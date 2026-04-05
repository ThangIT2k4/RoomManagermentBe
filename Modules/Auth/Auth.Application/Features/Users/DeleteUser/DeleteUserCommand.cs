using Auth.Application.Common;
using MediatR;

namespace Auth.Application.Features.Users.DeleteUser;

public sealed record DeleteUserCommand(Guid UserId, Guid DeletedBy) : IRequest<Result>;
