using Auth.Application.Common;
using Auth.Application.Dtos;
using Auth.Application.Services;
using RoomManagerment.Shared.Messaging;

namespace Auth.Application.Features.GetCurrentUser;

public sealed class GetCurrentUserQueryHandler(IAuthApplicationService authService)
    : IAppRequestHandler<GetCurrentUserQuery, Result<UserDto>>
{
    public Task<Result<UserDto>> Handle(GetCurrentUserQuery request, CancellationToken cancellationToken)
        => authService.GetCurrentUserAsync(new GetCurrentUserRequest(request.SessionId), cancellationToken);
}
