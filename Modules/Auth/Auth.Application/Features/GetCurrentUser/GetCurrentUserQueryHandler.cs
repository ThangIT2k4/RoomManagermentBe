using Auth.Application.Common;
using Auth.Application.Dtos;
using Auth.Application.Services;
using MediatR;

namespace Auth.Application.Features.GetCurrentUser;

public sealed class GetCurrentUserQueryHandler(IAuthApplicationService authService)
    : IRequestHandler<GetCurrentUserQuery, Result<UserDto>>
{
    public Task<Result<UserDto>> Handle(GetCurrentUserQuery request, CancellationToken cancellationToken)
        => authService.GetCurrentUserAsync(new GetCurrentUserRequest(request.SessionId), cancellationToken);
}
