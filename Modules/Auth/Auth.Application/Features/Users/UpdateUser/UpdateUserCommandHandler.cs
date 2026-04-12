using Auth.Application.Common;
using Auth.Application.Dtos;
using Auth.Application.Services;
using RoomManagerment.Shared.Messaging;

namespace Auth.Application.Features.Users.UpdateUser;

public sealed class UpdateUserCommandHandler(IAuthApplicationService authService)
    : IAppRequestHandler<UpdateUserCommand, Result<UserDto>>
{
    public Task<Result<UserDto>> Handle(UpdateUserCommand request, CancellationToken cancellationToken)
        => authService.UpdateUserAsync(
            new UpdateUserRequest(request.UserId, request.Email, request.Username, request.Phone, request.Status),
            cancellationToken);
}
