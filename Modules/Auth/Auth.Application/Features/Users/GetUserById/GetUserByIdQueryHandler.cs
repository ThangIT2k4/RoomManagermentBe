using Auth.Application.Common;
using Auth.Application.Dtos;
using Auth.Application.Services;
using MediatR;

namespace Auth.Application.Features.Auth.Users.GetUserById;

public sealed class GetUserByIdQueryHandler(IAuthApplicationService authService)
    : IRequestHandler<GetUserByIdQuery, Result<UserDto>>
{
    public Task<Result<UserDto>> Handle(GetUserByIdQuery request, CancellationToken cancellationToken)
        => authService.GetUserByIdAsync(new GetUserByIdRequest(request.UserId), cancellationToken);
}
