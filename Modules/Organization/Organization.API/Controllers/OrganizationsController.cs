using Microsoft.AspNetCore.Mvc;
using Organization.Application.Dtos;
using Organization.Application.Features.Banking.AddBanking;
using Organization.Application.Features.Banking.RemoveBanking;
using Organization.Application.Features.Banking.SetPrimaryBanking;
using Organization.Application.Features.Dashboard.GetDashboard;
using Organization.Application.Features.Invitations.AcceptInvitation;
using Organization.Application.Features.Invitations.CancelInvitation;
using Organization.Application.Features.Invitations.GetInvitation;
using Organization.Application.Features.Invitations.ResendInvitation;
using Organization.Application.Features.Invitations.SendInvitation;
using Organization.Application.Features.Members.CanMember;
using Organization.Application.Features.Members.ChangeMemberActivation;
using Organization.Application.Features.Members.ChangeMemberRole;
using Organization.Application.Features.Members.GetMembers;
using Organization.Application.Features.Members.RemoveMember;
using Organization.Application.Features.Members.UpsertMember;
using Organization.Application.Features.Organizations.GetOrganization;
using Organization.Application.Features.Organizations.OnboardOrganization;
using Organization.Application.Features.Organizations.UpdateOrganization;
using Organization.Application.Features.Quota.CheckQuota;
using Organization.Application.Features.Settings.UpsertEmailSettings;
using RoomManagerment.Shared.Messaging;

namespace Organization.API.Controllers;

[ApiController]
[Route("api/orgs")]
public sealed class OrganizationsController(IAppSender sender) : ControllerBase
{
    [HttpGet("{orgId:guid}")]
    public async Task<IActionResult> Get([FromRoute] Guid orgId, CancellationToken cancellationToken)
    {
        var result = await sender.Send(new GetOrganizationQuery(orgId), cancellationToken);
        return result.IsSuccess ? Ok(result.Value) : NotFound(result.Error);
    }

    [HttpPut("{orgId:guid}/onboard")]
    public async Task<IActionResult> Onboard([FromRoute] Guid orgId, [FromBody] OnboardOrganizationRequestBody body, CancellationToken cancellationToken)
    {
        var result = await sender.Send(
            new OnboardOrganizationCommand(orgId, body.ActorUserId, body.Name, body.Phone, body.Email, body.TaxCode, body.Address),
            cancellationToken);
        return result.IsSuccess ? Ok(result.Value) : BadRequest(result.Error);
    }

    [HttpPut("{orgId:guid}")]
    public async Task<IActionResult> Update([FromRoute] Guid orgId, [FromBody] UpdateOrganizationRequestBody body, CancellationToken cancellationToken)
    {
        var result = await sender.Send(
            new UpdateOrganizationCommand(orgId, body.ActorUserId, body.Name, body.Phone, body.Email, body.PublicMail, body.TaxCode, body.Address),
            cancellationToken);
        return result.IsSuccess ? Ok(result.Value) : BadRequest(result.Error);
    }

    [HttpGet("{orgId:guid}/members")]
    public async Task<IActionResult> GetMembers([FromRoute] Guid orgId, [FromQuery] int page = 1, [FromQuery] int pageSize = 20, CancellationToken cancellationToken = default)
    {
        if (!TryNormalizePaging(page, pageSize, out var normalizedPage, out var normalizedPageSize, out var pagingError))
            return BadRequest(new { error = pagingError });

        var result = await sender.Send(new GetMembersQuery(orgId, normalizedPage, normalizedPageSize), cancellationToken);
        return result.IsSuccess ? Ok(result.Value) : BadRequest(result.Error);
    }

    [HttpPost("{orgId:guid}/members")]
    public async Task<IActionResult> UpsertMember([FromRoute] Guid orgId, [FromBody] UpsertMemberRequestBody body, CancellationToken cancellationToken)
    {
        var result = await sender.Send(new UpsertMemberCommand(orgId, body.UserId, body.RoleId, body.ActorUserId), cancellationToken);
        return result.IsSuccess ? Ok(result.Value) : BadRequest(result.Error);
    }

    [HttpDelete("{orgId:guid}/members/{userId:guid}")]
    public async Task<IActionResult> RemoveMember([FromRoute] Guid orgId, [FromRoute] Guid userId, [FromBody] RemoveMemberRequestBody body, CancellationToken cancellationToken)
    {
        var result = await sender.Send(new RemoveMemberCommand(orgId, userId, body.ActorUserId, body.Reason), cancellationToken);
        return result.IsSuccess ? NoContent() : BadRequest(result.Error);
    }

    [HttpPut("{orgId:guid}/members/{userId:guid}/role")]
    public async Task<IActionResult> ChangeRole([FromRoute] Guid orgId, [FromRoute] Guid userId, [FromBody] ChangeMemberRoleRequestBody body, CancellationToken cancellationToken)
    {
        var result = await sender.Send(new ChangeMemberRoleCommand(orgId, userId, body.RoleId, body.ActorUserId), cancellationToken);
        return result.IsSuccess ? NoContent() : BadRequest(result.Error);
    }

    [HttpPost("{orgId:guid}/members/{userId:guid}/deactivate")]
    public async Task<IActionResult> Deactivate([FromRoute] Guid orgId, [FromRoute] Guid userId, [FromBody] ActivationRequestBody body, CancellationToken cancellationToken)
    {
        var result = await sender.Send(new ChangeMemberActivationCommand(orgId, userId, false, body.ActorUserId, body.Reason), cancellationToken);
        return result.IsSuccess ? NoContent() : BadRequest(result.Error);
    }

    [HttpPost("{orgId:guid}/members/{userId:guid}/activate")]
    public async Task<IActionResult> Activate([FromRoute] Guid orgId, [FromRoute] Guid userId, [FromBody] ActivationRequestBody body, CancellationToken cancellationToken)
    {
        var result = await sender.Send(new ChangeMemberActivationCommand(orgId, userId, true, body.ActorUserId, body.Reason), cancellationToken);
        return result.IsSuccess ? NoContent() : BadRequest(result.Error);
    }

    [HttpGet("{orgId:guid}/members/{userId:guid}/can/{capabilityKey}")]
    public async Task<IActionResult> Can([FromRoute] Guid orgId, [FromRoute] Guid userId, [FromRoute] string capabilityKey, CancellationToken cancellationToken)
    {
        var result = await sender.Send(new CanMemberQuery(orgId, userId, capabilityKey), cancellationToken);
        return result.IsSuccess ? Ok(new { allowed = result.Value }) : BadRequest(result.Error);
    }

    [HttpPost("{orgId:guid}/banking")]
    public async Task<IActionResult> AddBanking([FromRoute] Guid orgId, [FromBody] AddBankingRequestBody body, CancellationToken cancellationToken)
    {
        var result = await sender.Send(
            new AddOrganizationBankingCommand(orgId, body.ActorUserId, body.SepayBankId, body.AccountNumber, body.AccountHolderName, body.BranchName, body.BranchCode, body.SwiftCode, body.IsPrimary),
            cancellationToken);
        return result.IsSuccess ? Ok(result.Value) : BadRequest(result.Error);
    }

    [HttpPost("{orgId:guid}/banking/{bankingId:guid}/set-primary")]
    public async Task<IActionResult> SetPrimaryBanking([FromRoute] Guid orgId, [FromRoute] Guid bankingId, [FromBody] ActorBody body, CancellationToken cancellationToken)
    {
        var result = await sender.Send(new SetPrimaryBankingCommand(orgId, bankingId, body.ActorUserId), cancellationToken);
        return result.IsSuccess ? NoContent() : BadRequest(result.Error);
    }

    [HttpDelete("{orgId:guid}/banking/{bankingId:guid}")]
    public async Task<IActionResult> RemoveBanking([FromRoute] Guid orgId, [FromRoute] Guid bankingId, [FromBody] ActorBody body, CancellationToken cancellationToken)
    {
        var result = await sender.Send(new RemoveBankingCommand(orgId, bankingId, body.ActorUserId), cancellationToken);
        return result.IsSuccess ? NoContent() : BadRequest(result.Error);
    }

    [HttpPost("{orgId:guid}/invitations")]
    public async Task<IActionResult> SendInvitation([FromRoute] Guid orgId, [FromBody] SendInvitationRequestBody body, CancellationToken cancellationToken)
    {
        var result = await sender.Send(new SendInvitationCommand(orgId, body.Email, body.RoleId, body.ActorUserId, body.Note), cancellationToken);
        return result.IsSuccess ? Ok(result.Value) : BadRequest(result.Error);
    }

    [HttpDelete("{orgId:guid}/invitations/{membershipId:guid}")]
    public async Task<IActionResult> CancelInvitation([FromRoute] Guid orgId, [FromRoute] Guid membershipId, [FromBody] ActorBody body, CancellationToken cancellationToken)
    {
        var result = await sender.Send(new CancelInvitationCommand(orgId, membershipId, body.ActorUserId), cancellationToken);
        return result.IsSuccess ? NoContent() : BadRequest(result.Error);
    }

    [HttpPost("{orgId:guid}/invitations/{membershipId:guid}/resend")]
    public async Task<IActionResult> ResendInvitation([FromRoute] Guid orgId, [FromRoute] Guid membershipId, [FromBody] ActorBody body, CancellationToken cancellationToken)
    {
        var result = await sender.Send(new ResendInvitationCommand(orgId, membershipId, body.ActorUserId), cancellationToken);
        return result.IsSuccess ? Ok(result.Value) : BadRequest(result.Error);
    }

    [HttpGet("invitations/{token}")]
    public async Task<IActionResult> GetInvitation([FromRoute] string token, CancellationToken cancellationToken)
    {
        var result = await sender.Send(new GetInvitationQuery(token), cancellationToken);
        return result.IsSuccess ? Ok(result.Value) : NotFound(result.Error);
    }

    [HttpPost("invitations/{token}/accept-existing")]
    public async Task<IActionResult> AcceptInvitation([FromRoute] string token, [FromBody] AcceptInvitationBody body, CancellationToken cancellationToken)
    {
        var result = await sender.Send(new AcceptInvitationCommand(token, body.UserId), cancellationToken);
        return result.IsSuccess ? NoContent() : BadRequest(result.Error);
    }

    [HttpPut("{orgId:guid}/email-settings")]
    public async Task<IActionResult> UpsertEmailSettings([FromRoute] Guid orgId, [FromBody] UpsertEmailSettingsBody body, CancellationToken cancellationToken)
    {
        var result = await sender.Send(
            new UpsertEmailSettingsCommand(orgId, body.ActorUserId, body.FromName, body.FromEmail, body.Provider, body.SmtpHost, body.SmtpPort, body.SmtpEncryption, body.SmtpUsername, body.SmtpPassword),
            cancellationToken);
        return result.IsSuccess ? NoContent() : BadRequest(result.Error);
    }

    [HttpGet("{orgId:guid}/quota/{featureKey}")]
    public async Task<IActionResult> Quota([FromRoute] Guid orgId, [FromRoute] string featureKey, [FromQuery] int currentUsage = 0, CancellationToken cancellationToken = default)
    {
        if (currentUsage < 0)
            return BadRequest(new { error = "CurrentUsage must be greater than or equal to 0." });

        var normalizedFeatureKey = featureKey.Trim();
        if (string.IsNullOrWhiteSpace(normalizedFeatureKey))
            return BadRequest(new { error = "FeatureKey is required." });

        var result = await sender.Send(new CheckQuotaQuery(orgId, normalizedFeatureKey, currentUsage), cancellationToken);
        return result.IsSuccess ? Ok(result.Value) : BadRequest(result.Error);
    }

    [HttpGet("{orgId:guid}/dashboard")]
    public async Task<IActionResult> Dashboard([FromRoute] Guid orgId, CancellationToken cancellationToken)
    {
        var result = await sender.Send(new GetDashboardQuery(orgId), cancellationToken);
        return result.IsSuccess ? Ok(result.Value) : BadRequest(result.Error);
    }

    private static bool TryNormalizePaging(int page, int pageSize, out int normalizedPage, out int normalizedPageSize, out string? error)
    {
        normalizedPage = page;
        normalizedPageSize = pageSize;
        error = null;

        if (page < 1)
        {
            error = "Page must be greater than or equal to 1.";
            return false;
        }

        if (pageSize < 1 || pageSize > 200)
        {
            error = "PageSize must be between 1 and 200.";
            return false;
        }

        return true;
    }
}

public sealed record ActorBody(Guid ActorUserId);
public sealed record OnboardOrganizationRequestBody(Guid ActorUserId, string Name, string? Phone, string? Email, string? TaxCode, string? Address);
public sealed record UpdateOrganizationRequestBody(Guid ActorUserId, string Name, string? Phone, string? Email, string? PublicMail, string? TaxCode, string? Address);
public sealed record AddBankingRequestBody(Guid ActorUserId, Guid? SepayBankId, string AccountNumber, string AccountHolderName, string? BranchName, string? BranchCode, string? SwiftCode, bool IsPrimary);
public sealed record UpsertMemberRequestBody(Guid ActorUserId, Guid UserId, Guid RoleId);
public sealed record RemoveMemberRequestBody(Guid ActorUserId, string? Reason);
public sealed record ChangeMemberRoleRequestBody(Guid ActorUserId, Guid RoleId);
public sealed record ActivationRequestBody(Guid ActorUserId, string? Reason);
public sealed record SendInvitationRequestBody(Guid ActorUserId, string Email, Guid RoleId, string? Note);
public sealed record AcceptInvitationBody(Guid UserId);
public sealed record UpsertEmailSettingsBody(Guid ActorUserId, string FromName, string FromEmail, string Provider, string? SmtpHost, int? SmtpPort, string? SmtpEncryption, string? SmtpUsername, string? SmtpPassword);
