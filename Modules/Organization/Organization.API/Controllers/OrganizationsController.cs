using Microsoft.AspNetCore.Mvc;
using Organization.Application.Dtos;
using Organization.Application.Services;

namespace Organization.API.Controllers;

[ApiController]
[Route("api/orgs")]
public sealed class OrganizationsController(IOrganizationApplicationService service) : ControllerBase
{
    [HttpGet("{orgId:guid}")]
    public async Task<IActionResult> Get([FromRoute] Guid orgId, CancellationToken cancellationToken)
    {
        var result = await service.GetOrganizationAsync(orgId, cancellationToken);
        return result.IsSuccess ? Ok(result.Value) : NotFound(result.Error);
    }

    [HttpPut("{orgId:guid}/onboard")]
    public async Task<IActionResult> Onboard([FromRoute] Guid orgId, [FromBody] OnboardOrganizationRequestBody body, CancellationToken cancellationToken)
    {
        var result = await service.OnboardOrganizationAsync(
            new OnboardOrganizationRequest(orgId, body.ActorUserId, body.Name, body.Phone, body.Email, body.TaxCode, body.Address),
            cancellationToken);
        return result.IsSuccess ? Ok(result.Value) : BadRequest(result.Error);
    }

    [HttpPut("{orgId:guid}")]
    public async Task<IActionResult> Update([FromRoute] Guid orgId, [FromBody] UpdateOrganizationRequestBody body, CancellationToken cancellationToken)
    {
        var result = await service.UpdateOrganizationAsync(
            new UpdateOrganizationRequest(orgId, body.ActorUserId, body.Name, body.Phone, body.Email, body.PublicMail, body.TaxCode, body.Address),
            cancellationToken);
        return result.IsSuccess ? Ok(result.Value) : BadRequest(result.Error);
    }

    [HttpGet("{orgId:guid}/members")]
    public async Task<IActionResult> GetMembers([FromRoute] Guid orgId, [FromQuery] int page = 1, [FromQuery] int pageSize = 20, CancellationToken cancellationToken = default)
    {
        var result = await service.GetMembersAsync(orgId, page, pageSize, cancellationToken);
        return result.IsSuccess ? Ok(result.Value) : BadRequest(result.Error);
    }

    [HttpPost("{orgId:guid}/members")]
    public async Task<IActionResult> UpsertMember([FromRoute] Guid orgId, [FromBody] UpsertMemberRequestBody body, CancellationToken cancellationToken)
    {
        var result = await service.UpsertMemberAsync(new UpsertMemberRequest(orgId, body.UserId, body.RoleId, body.ActorUserId), cancellationToken);
        return result.IsSuccess ? Ok(result.Value) : BadRequest(result.Error);
    }

    [HttpDelete("{orgId:guid}/members/{userId:guid}")]
    public async Task<IActionResult> RemoveMember([FromRoute] Guid orgId, [FromRoute] Guid userId, [FromBody] RemoveMemberRequestBody body, CancellationToken cancellationToken)
    {
        var result = await service.RemoveMemberAsync(new RemoveMemberRequest(orgId, userId, body.ActorUserId, body.Reason), cancellationToken);
        return result.IsSuccess ? NoContent() : BadRequest(result.Error);
    }

    [HttpPut("{orgId:guid}/members/{userId:guid}/role")]
    public async Task<IActionResult> ChangeRole([FromRoute] Guid orgId, [FromRoute] Guid userId, [FromBody] ChangeMemberRoleRequestBody body, CancellationToken cancellationToken)
    {
        var result = await service.ChangeMemberRoleAsync(new ChangeMemberRoleRequest(orgId, userId, body.RoleId, body.ActorUserId), cancellationToken);
        return result.IsSuccess ? NoContent() : BadRequest(result.Error);
    }

    [HttpPost("{orgId:guid}/members/{userId:guid}/deactivate")]
    public async Task<IActionResult> Deactivate([FromRoute] Guid orgId, [FromRoute] Guid userId, [FromBody] ActivationRequestBody body, CancellationToken cancellationToken)
    {
        var result = await service.ChangeMemberActivationAsync(new ChangeMemberActivationRequest(orgId, userId, false, body.ActorUserId, body.Reason), cancellationToken);
        return result.IsSuccess ? NoContent() : BadRequest(result.Error);
    }

    [HttpPost("{orgId:guid}/members/{userId:guid}/activate")]
    public async Task<IActionResult> Activate([FromRoute] Guid orgId, [FromRoute] Guid userId, [FromBody] ActivationRequestBody body, CancellationToken cancellationToken)
    {
        var result = await service.ChangeMemberActivationAsync(new ChangeMemberActivationRequest(orgId, userId, true, body.ActorUserId, body.Reason), cancellationToken);
        return result.IsSuccess ? NoContent() : BadRequest(result.Error);
    }

    [HttpGet("{orgId:guid}/members/{userId:guid}/can/{capabilityKey}")]
    public async Task<IActionResult> Can([FromRoute] Guid orgId, [FromRoute] Guid userId, [FromRoute] string capabilityKey, CancellationToken cancellationToken)
    {
        var result = await service.CanMemberAsync(new CanMemberRequest(orgId, userId, capabilityKey), cancellationToken);
        return result.IsSuccess ? Ok(new { allowed = result.Value }) : BadRequest(result.Error);
    }

    [HttpPost("{orgId:guid}/banking")]
    public async Task<IActionResult> AddBanking([FromRoute] Guid orgId, [FromBody] AddBankingRequestBody body, CancellationToken cancellationToken)
    {
        var result = await service.AddBankingAsync(
            new AddOrganizationBankingRequest(orgId, body.ActorUserId, body.SepayBankId, body.AccountNumber, body.AccountHolderName, body.BranchName, body.BranchCode, body.SwiftCode, body.IsPrimary),
            cancellationToken);
        return result.IsSuccess ? Ok(result.Value) : BadRequest(result.Error);
    }

    [HttpPost("{orgId:guid}/banking/{bankingId:guid}/set-primary")]
    public async Task<IActionResult> SetPrimaryBanking([FromRoute] Guid orgId, [FromRoute] Guid bankingId, [FromBody] ActorBody body, CancellationToken cancellationToken)
    {
        var result = await service.SetPrimaryBankingAsync(new SetPrimaryBankingRequest(orgId, bankingId, body.ActorUserId), cancellationToken);
        return result.IsSuccess ? NoContent() : BadRequest(result.Error);
    }

    [HttpDelete("{orgId:guid}/banking/{bankingId:guid}")]
    public async Task<IActionResult> RemoveBanking([FromRoute] Guid orgId, [FromRoute] Guid bankingId, [FromBody] ActorBody body, CancellationToken cancellationToken)
    {
        var result = await service.RemoveBankingAsync(new RemoveBankingRequest(orgId, bankingId, body.ActorUserId), cancellationToken);
        return result.IsSuccess ? NoContent() : BadRequest(result.Error);
    }

    [HttpPost("{orgId:guid}/invitations")]
    public async Task<IActionResult> SendInvitation([FromRoute] Guid orgId, [FromBody] SendInvitationRequestBody body, CancellationToken cancellationToken)
    {
        var result = await service.SendInvitationAsync(new SendInvitationRequest(orgId, body.Email, body.RoleId, body.ActorUserId, body.Note), cancellationToken);
        return result.IsSuccess ? Ok(result.Value) : BadRequest(result.Error);
    }

    [HttpDelete("{orgId:guid}/invitations/{membershipId:guid}")]
    public async Task<IActionResult> CancelInvitation([FromRoute] Guid orgId, [FromRoute] Guid membershipId, [FromBody] ActorBody body, CancellationToken cancellationToken)
    {
        var result = await service.CancelInvitationAsync(new CancelInvitationRequest(orgId, membershipId, body.ActorUserId), cancellationToken);
        return result.IsSuccess ? NoContent() : BadRequest(result.Error);
    }

    [HttpPost("{orgId:guid}/invitations/{membershipId:guid}/resend")]
    public async Task<IActionResult> ResendInvitation([FromRoute] Guid orgId, [FromRoute] Guid membershipId, [FromBody] ActorBody body, CancellationToken cancellationToken)
    {
        var result = await service.ResendInvitationAsync(new ResendInvitationRequest(orgId, membershipId, body.ActorUserId), cancellationToken);
        return result.IsSuccess ? Ok(result.Value) : BadRequest(result.Error);
    }

    [HttpGet("invitations/{token}")]
    public async Task<IActionResult> GetInvitation([FromRoute] string token, CancellationToken cancellationToken)
    {
        var result = await service.GetInvitationAsync(token, cancellationToken);
        return result.IsSuccess ? Ok(result.Value) : NotFound(result.Error);
    }

    [HttpPost("invitations/{token}/accept-existing")]
    public async Task<IActionResult> AcceptInvitation([FromRoute] string token, [FromBody] AcceptInvitationBody body, CancellationToken cancellationToken)
    {
        var result = await service.AcceptInvitationAsync(new AcceptInvitationRequest(token, body.UserId), cancellationToken);
        return result.IsSuccess ? NoContent() : BadRequest(result.Error);
    }

    [HttpPut("{orgId:guid}/email-settings")]
    public async Task<IActionResult> UpsertEmailSettings([FromRoute] Guid orgId, [FromBody] UpsertEmailSettingsBody body, CancellationToken cancellationToken)
    {
        var result = await service.UpsertEmailSettingsAsync(
            new UpsertEmailSettingsRequest(orgId, body.ActorUserId, body.FromName, body.FromEmail, body.Provider, body.SmtpHost, body.SmtpPort, body.SmtpEncryption, body.SmtpUsername, body.SmtpPassword),
            cancellationToken);
        return result.IsSuccess ? NoContent() : BadRequest(result.Error);
    }

    [HttpGet("{orgId:guid}/quota/{featureKey}")]
    public async Task<IActionResult> Quota([FromRoute] Guid orgId, [FromRoute] string featureKey, [FromQuery] int currentUsage = 0, CancellationToken cancellationToken = default)
    {
        var result = await service.CheckQuotaAsync(new CheckQuotaRequest(orgId, featureKey, currentUsage), cancellationToken);
        return result.IsSuccess ? Ok(result.Value) : BadRequest(result.Error);
    }

    [HttpGet("{orgId:guid}/dashboard")]
    public async Task<IActionResult> Dashboard([FromRoute] Guid orgId, CancellationToken cancellationToken)
    {
        var result = await service.GetDashboardAsync(orgId, cancellationToken);
        return result.IsSuccess ? Ok(result.Value) : BadRequest(result.Error);
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
