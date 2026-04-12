using System.Security.Claims;
using Auth.API.Requests;
using Auth.Application.Features.Auth.Profile.GetProfile;
using Auth.Application.Features.Auth.Profile.UpdatePersonalInfo;
using Auth.Application.Features.Auth.Profile.UpdateProfile;
using Auth.Application.Features.Auth.Profile.UploadAvatar;
using Auth.Application.Dtos;
using RoomManagerment.Shared.Http;
using RoomManagerment.Shared.Messaging;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RoomManagerment.Shared.Extensions;


namespace Auth.API.Controllers;

[ApiController]
[Authorize]
[Route("api/profile")]
public sealed class ProfileController(IAppSender sender) : ControllerBase
{
    [HttpGet]
    [ProducesResponseType(typeof(ApiResponse<UserProfileDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<UserProfileDto>), StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<ApiResponse<UserProfileDto>>> GetProfile(CancellationToken cancellationToken)
    {
        var userId = ResolveUserId();
        if (userId is null)
        {
            return this.ApiUnauthorized<UserProfileDto>("User identity is missing.");
        }

        var result = await sender.Send(new GetProfileQuery(userId.Value), cancellationToken);
        return this.ToApiActionResult(result);
    }

    [HttpPut]
    [ProducesResponseType(typeof(ApiResponse<UserProfileDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<UserProfileDto>), StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<ApiResponse<UserProfileDto>>> UpdateProfile([FromBody] UpdateProfileApiRequest request, CancellationToken cancellationToken)
    {
        var userId = ResolveUserId();
        if (userId is null)
        {
            return this.ApiUnauthorized<UserProfileDto>("User identity is missing.");
        }

        var result = await sender.Send(
            new UpdateProfileCommand(userId.Value, request.FullName, request.Dob, request.Gender, request.Address, request.Note),
            cancellationToken);
        return this.ToApiActionResult(result);
    }

    [HttpPatch("avatar")]
    [ProducesResponseType(typeof(ApiResponse<UserProfileDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<UserProfileDto>), StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<ApiResponse<UserProfileDto>>> UploadAvatar([FromBody] UploadAvatarApiRequest request, CancellationToken cancellationToken)
    {
        var userId = ResolveUserId();
        if (userId is null)
        {
            return this.ApiUnauthorized<UserProfileDto>("User identity is missing.");
        }

        var result = await sender.Send(new UploadAvatarCommand(userId.Value, request.AvatarUrl), cancellationToken);
        return this.ToApiActionResult(result);
    }

    [HttpPatch("personal-info")]
    [ProducesResponseType(typeof(ApiResponse<UserProfileDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<UserProfileDto>), StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<ApiResponse<UserProfileDto>>> UpdatePersonalInfo([FromBody] UpdateProfileApiRequest request, CancellationToken cancellationToken)
    {
        var userId = ResolveUserId();
        if (userId is null)
        {
            return this.ApiUnauthorized<UserProfileDto>("User identity is missing.");
        }

        var result = await sender.Send(
            new UpdatePersonalInfoCommand(userId.Value, request.FullName, request.Dob, request.Gender, request.Address, request.Note),
            cancellationToken);
        return this.ToApiActionResult(result);
    }

    private Guid? ResolveUserId()
    {
        var raw = User.FindFirstValue(ClaimTypes.NameIdentifier);
        return Guid.TryParse(raw, out var userId) ? userId : null;
    }
}
