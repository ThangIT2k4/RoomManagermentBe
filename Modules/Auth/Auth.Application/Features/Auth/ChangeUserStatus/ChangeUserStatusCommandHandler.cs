using Auth.Application.Common;
using Auth.Application.Dtos;
using Auth.Application.Services;
using MediatR;

namespace Auth.Application.Features.Auth.ChangeUserStatus;

public sealed class ChangeUserStatusCommandHandler(IAuthApplicationService authService)
    : IRequestHandler<ChangeUserStatusCommand, Result<UserDto>>
{
    public Task<Result<UserDto>> Handle(ChangeUserStatusCommand request, CancellationToken cancellationToken)
        => authService.ChangeUserStatusAsync(new ChangeUserStatusRequest(request.UserId, request.Status), cancellationToken);
}
