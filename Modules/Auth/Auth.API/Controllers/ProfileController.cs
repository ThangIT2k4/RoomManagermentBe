using System.Security.Claims;
using Auth.API.Common;
using Auth.API.Requests;
using Auth.Application.Dtos;
using Auth.Application.Services;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Auth.API.Controllers;

[ApiController]
[Authorize]
[Route("api/profile")]
public sealed class ProfileController(
    IAuthApplicationService authService,
    IValidator<UpdateProfileApiRequest> updateProfileValidator,
    IValidator<UploadAvatarApiRequest> uploadAvatarValidator) : ControllerBase
{
    [HttpGet]
    [ProducesResponseType(typeof(UserProfileDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<UserProfileDto>> GetProfile(CancellationToken cancellationToken)
    {
        var userId = ResolveUserId();
        if (userId is null)
        {
            return Unauthorized(new { message = "User identity is missing." });
        }

        var result = await authService.GetProfileAsync(new GetProfileRequest(userId.Value), cancellationToken);
        return result.ToActionResult();
    }

    [HttpPut]
    [ProducesResponseType(typeof(UserProfileDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<UserProfileDto>> UpdateProfile([FromBody] UpdateProfileApiRequest request, CancellationToken cancellationToken)
    {
        var userId = ResolveUserId();
        if (userId is null)
        {
            return Unauthorized(new { message = "User identity is missing." });
        }

        var validation = await updateProfileValidator.ValidateAsync(request, cancellationToken);
        if (!validation.IsValid)
        {
            return BadRequest(ToValidationErrorPayload(validation));
        }

        var command = new UpdateProfileRequest(userId.Value, request.FullName, request.Dob, request.Gender, request.Address, request.Note);
        var result = await authService.UpdateProfileAsync(command, cancellationToken);
        return result.ToActionResult();
    }

    [HttpPatch("avatar")]
    [ProducesResponseType(typeof(UserProfileDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<UserProfileDto>> UploadAvatar([FromBody] UploadAvatarApiRequest request, CancellationToken cancellationToken)
    {
        var userId = ResolveUserId();
        if (userId is null)
        {
            return Unauthorized(new { message = "User identity is missing." });
        }

        var validation = await uploadAvatarValidator.ValidateAsync(request, cancellationToken);
        if (!validation.IsValid)
        {
            return BadRequest(ToValidationErrorPayload(validation));
        }

        var result = await authService.UploadAvatarAsync(new UploadAvatarRequest(userId.Value, request.AvatarUrl), cancellationToken);
        return result.ToActionResult();
    }

    [HttpPatch("personal-info")]
    [ProducesResponseType(typeof(UserProfileDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<UserProfileDto>> UpdatePersonalInfo([FromBody] UpdateProfileApiRequest request, CancellationToken cancellationToken)
    {
        var userId = ResolveUserId();
        if (userId is null)
        {
            return Unauthorized(new { message = "User identity is missing." });
        }

        var validation = await updateProfileValidator.ValidateAsync(request, cancellationToken);
        if (!validation.IsValid)
        {
            return BadRequest(ToValidationErrorPayload(validation));
        }

        var command = new UpdatePersonalInfoRequest(userId.Value, request.FullName, request.Dob, request.Gender, request.Address, request.Note);
        var result = await authService.UpdatePersonalInfoAsync(command, cancellationToken);
        return result.ToActionResult();
    }

    private Guid? ResolveUserId()
    {
        var raw = User.FindFirstValue(ClaimTypes.NameIdentifier);
        return Guid.TryParse(raw, out var userId) ? userId : null;
    }

    private static object ToValidationErrorPayload(ValidationResult validationResult)
    {
        return new
        {
            message = "Validation failed",
            errors = validationResult.Errors.Select(error => new
            {
                field = error.PropertyName,
                error = error.ErrorMessage
            })
        };
    }
}
