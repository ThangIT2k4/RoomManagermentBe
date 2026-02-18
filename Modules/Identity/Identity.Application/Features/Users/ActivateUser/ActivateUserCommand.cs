using Identity.Application.Common;
using MediatR;

namespace Identity.Application.Features.Users.ActivateUser;

public sealed record ActivateUserCommand(Guid UserId) : IRequest<Result>;

