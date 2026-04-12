using Auth.Application.Common;
using Auth.Application.Dtos;
using Auth.Application.Services;
using RoomManagerment.Shared.Messaging;

namespace Auth.Application.Features.Auth.Users.GetUserById;

public sealed class GetUserByIdQueryHandler(IAuthApplicationService authService)
    : IAppRequestHandler<GetUserByIdQuery, Result<UserDto>>
{
    public Task<Result<UserDto>> Handle(GetUserByIdQuery request, CancellationToken cancellationToken)
        => authService.GetUserByIdAsync(new GetUserByIdRequest(request.UserId), cancellationToken);
}
