using Auth.Application.Common;
using Auth.Application.Dtos;
using Auth.Application.Services;
using RoomManagerment.Shared.Messaging;

namespace Auth.Application.Features.Auth.Profile.UpdateProfile;

public sealed class UpdateProfileCommandHandler(IAuthApplicationService authService)
    : IAppRequestHandler<UpdateProfileCommand, Result<UserProfileDto>>
{
    public Task<Result<UserProfileDto>> Handle(UpdateProfileCommand request, CancellationToken cancellationToken)
        => authService.UpdateProfileAsync(new UpdateProfileRequest(request.UserId, request.FullName, request.Dob, request.Gender, request.Address, request.Note), cancellationToken);
}
