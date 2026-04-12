using Auth.Application.Common;
using Auth.Application.Dtos;
using Auth.Application.Services;
using RoomManagerment.Shared.Messaging;

namespace Auth.Application.Features.Auth.Profile.UpdatePersonalInfo;

public sealed class UpdatePersonalInfoCommandHandler(IAuthApplicationService authService)
    : IAppRequestHandler<UpdatePersonalInfoCommand, Result<UserProfileDto>>
{
    public Task<Result<UserProfileDto>> Handle(UpdatePersonalInfoCommand request, CancellationToken cancellationToken)
        => authService.UpdatePersonalInfoAsync(new UpdatePersonalInfoRequest(request.UserId, request.FullName, request.Dob, request.Gender, request.Address, request.Note), cancellationToken);
}
