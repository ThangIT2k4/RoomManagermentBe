using Auth.Application.Common;
using Auth.Application.Dtos;
using Auth.Application.Services;
using RoomManagerment.Shared.Messaging;

namespace Auth.Application.Features.Auth.ChangeUserStatus;

public sealed class ChangeUserStatusCommandHandler(IAuthApplicationService authService)
    : IAppRequestHandler<ChangeUserStatusCommand, Result<UserDto>>
{
    public Task<Result<UserDto>> Handle(ChangeUserStatusCommand request, CancellationToken cancellationToken)
        => authService.ChangeUserStatusAsync(new ChangeUserStatusRequest(request.UserId, request.Status), cancellationToken);
}
