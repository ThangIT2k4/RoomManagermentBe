using Auth.Application.Common;
using Auth.Application.Dtos;
using Auth.Application.Services;
using RoomManagerment.Shared.Messaging;

namespace Auth.Application.Features.Auth.Profile.GetProfile;

public sealed class GetProfileQueryHandler(IAuthApplicationService authService)
    : IAppRequestHandler<GetProfileQuery, Result<UserProfileDto>>
{
    public Task<Result<UserProfileDto>> Handle(GetProfileQuery request, CancellationToken cancellationToken)
        => authService.GetProfileAsync(new GetProfileRequest(request.UserId), cancellationToken);
}
