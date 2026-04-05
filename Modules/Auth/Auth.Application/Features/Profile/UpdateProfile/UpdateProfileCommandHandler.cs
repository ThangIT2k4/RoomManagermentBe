using Auth.Application.Common;
using Auth.Application.Dtos;
using Auth.Application.Services;
using MediatR;

namespace Auth.Application.Features.Auth.Profile.UpdateProfile;

public sealed class UpdateProfileCommandHandler(IAuthApplicationService authService)
    : IRequestHandler<UpdateProfileCommand, Result<UserProfileDto>>
{
    public Task<Result<UserProfileDto>> Handle(UpdateProfileCommand request, CancellationToken cancellationToken)
        => authService.UpdateProfileAsync(new UpdateProfileRequest(request.UserId, request.FullName, request.Dob, request.Gender, request.Address, request.Note), cancellationToken);
}
