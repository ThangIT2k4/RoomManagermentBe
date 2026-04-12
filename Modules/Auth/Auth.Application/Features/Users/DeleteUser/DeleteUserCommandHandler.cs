using Auth.Application.Common;
using Auth.Application.Dtos;
using Auth.Application.Services;
using RoomManagerment.Shared.Messaging;

namespace Auth.Application.Features.Users.DeleteUser;

public sealed class DeleteUserCommandHandler(IAuthApplicationService authService)
    : IAppRequestHandler<DeleteUserCommand, Result>
{
    public Task<Result> Handle(DeleteUserCommand request, CancellationToken cancellationToken)
        => authService.DeleteUserAsync(new DeleteUserRequest(request.UserId, request.DeletedBy), cancellationToken);
}
