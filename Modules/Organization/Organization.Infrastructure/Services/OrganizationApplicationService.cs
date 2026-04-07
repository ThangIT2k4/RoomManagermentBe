using Organization.Application.Common;
using Organization.Application.Dtos;
using Organization.Application.Services;
using Organization.Domain.Entities;
using Organization.Domain.Events;
using Organization.Domain.Repositories;

namespace Organization.Infrastructure.Services;

public sealed class OrganizationApplicationService(
    IOrganizationRepository organizationRepository,
    IOrganizationUserRepository organizationUserRepository,
    IOrganizationBankingRepository organizationBankingRepository,
    IOrganizationUserCapabilityRepository organizationUserCapabilityRepository,
    IOrganizationCacheService cacheService,
    IIntegrationEventPublisher eventPublisher) : IOrganizationApplicationService
{
    public async Task<Result<OrganizationDto>> GetOrganizationAsync(Guid organizationId, CancellationToken cancellationToken = default)
    {
        var cached = await cacheService.GetOrganizationAsync(organizationId, cancellationToken);
        if (cached is not null)
        {
            return Result<OrganizationDto>.Success(cached);
        }

        var org = await organizationRepository.GetByIdAsync(organizationId, cancellationToken);
        if (org is null)
        {
            return Result<OrganizationDto>.Failure(new Error("Org.NotFound", "Organization not found."));
        }

        var dto = Map(org);
        await cacheService.SetOrganizationAsync(dto, TimeSpan.FromMinutes(5), cancellationToken);
        return Result<OrganizationDto>.Success(dto);
    }

    public async Task<Result<OrganizationDto>> OnboardOrganizationAsync(OnboardOrganizationRequest request, CancellationToken cancellationToken = default)
    {
        var org = await organizationRepository.GetByIdAsync(request.OrganizationId, cancellationToken);
        if (org is null)
        {
            return Result<OrganizationDto>.Failure(new Error("Org.NotFound", "Organization not found."));
        }

        org.UpdateProfile(request.Name, request.Phone, request.Email, null, request.TaxCode, request.Address, DateTime.UtcNow);
        await organizationRepository.UpdateAsync(org, cancellationToken);
        await cacheService.RemoveOrganizationAsync(org.Id, cancellationToken);
        await eventPublisher.EnqueueAsync(new { org_id = org.Id }, "org.onboarded", cancellationToken);
        return Result<OrganizationDto>.Success(Map(org));
    }

    public async Task<Result<OrganizationDto>> UpdateOrganizationAsync(UpdateOrganizationRequest request, CancellationToken cancellationToken = default)
    {
        var org = await organizationRepository.GetByIdAsync(request.OrganizationId, cancellationToken);
        if (org is null)
        {
            return Result<OrganizationDto>.Failure(new Error("Org.NotFound", "Organization not found."));
        }

        org.UpdateProfile(request.Name, request.Phone, request.Email, request.PublicMail, request.TaxCode, request.Address, DateTime.UtcNow);
        await organizationRepository.UpdateAsync(org, cancellationToken);
        await cacheService.RemoveOrganizationAsync(org.Id, cancellationToken);
        await eventPublisher.EnqueueAsync(new OrganizationUpdatedEvent(org.Id, DateTime.UtcNow), "org.updated", cancellationToken);
        return Result<OrganizationDto>.Success(Map(org));
    }

    public async Task<Result<OrganizationBankingDto>> AddBankingAsync(AddOrganizationBankingRequest request, CancellationToken cancellationToken = default)
    {
        var list = await organizationBankingRepository.GetByOrganizationAsync(request.OrganizationId, cancellationToken);
        if (list.Any(x => x.AccountNumber == request.AccountNumber))
        {
            return Result<OrganizationBankingDto>.Failure(new Error("Org.Banking.Duplicate", "Account number already exists in organization."));
        }

        var banking = OrganizationBankingEntity.Create(
            request.OrganizationId,
            request.SepayBankId,
            request.AccountNumber,
            request.AccountHolderName,
            request.BranchName,
            request.BranchCode,
            request.SwiftCode,
            request.IsPrimary || list.Count == 0,
            DateTime.UtcNow);

        await organizationBankingRepository.AddAsync(banking, cancellationToken);
        if (banking.IsPrimary)
        {
            foreach (var item in list.Where(x => x.Id != banking.Id && x.IsPrimary))
            {
                item.SetPrimary(false);
                await organizationBankingRepository.UpdateAsync(item, cancellationToken);
            }
        }

        await eventPublisher.EnqueueAsync(new { org_id = request.OrganizationId }, "org.banking_updated", cancellationToken);
        return Result<OrganizationBankingDto>.Success(Map(banking));
    }

    public async Task<Result> SetPrimaryBankingAsync(SetPrimaryBankingRequest request, CancellationToken cancellationToken = default)
    {
        var list = await organizationBankingRepository.GetByOrganizationAsync(request.OrganizationId, cancellationToken);
        foreach (var item in list)
        {
            item.SetPrimary(item.Id == request.BankingId);
            await organizationBankingRepository.UpdateAsync(item, cancellationToken);
        }

        await eventPublisher.EnqueueAsync(new { org_id = request.OrganizationId }, "org.banking_updated", cancellationToken);
        return Result.Success();
    }

    public async Task<Result> RemoveBankingAsync(RemoveBankingRequest request, CancellationToken cancellationToken = default)
    {
        await organizationBankingRepository.SoftDeleteAsync(request.BankingId, request.ActorUserId, DateTime.UtcNow, cancellationToken);
        await eventPublisher.EnqueueAsync(new { org_id = request.OrganizationId }, "org.banking_updated", cancellationToken);
        return Result.Success();
    }

    public async Task<Result<PagedResponse<OrganizationMemberDto>>> GetMembersAsync(Guid organizationId, int page, int pageSize, CancellationToken cancellationToken = default)
    {
        var safePage = Math.Max(1, page);
        var safePageSize = Math.Clamp(pageSize, 1, 200);
        var items = await organizationUserRepository.GetPagedByOrgAsync(organizationId, safePage, safePageSize, cancellationToken);
        var total = await organizationUserRepository.CountByOrgAsync(organizationId, cancellationToken);
        var totalPages = total == 0 ? 0 : (int)Math.Ceiling(total / (double)safePageSize);
        return Result<PagedResponse<OrganizationMemberDto>>.Success(
            new PagedResponse<OrganizationMemberDto>(
                items.Select(Map).ToList(),
                total,
                safePage,
                safePageSize,
                totalPages,
                safePage > 1,
                safePage < totalPages));
    }

    public async Task<Result<OrganizationMemberDto>> UpsertMemberAsync(UpsertMemberRequest request, CancellationToken cancellationToken = default)
    {
        var existing = await organizationUserRepository.GetByOrgAndUserAsync(request.OrganizationId, request.UserId, cancellationToken);
        if (existing is null)
        {
            var created = OrganizationUserEntity.Create(request.OrganizationId, request.UserId, request.RoleId, DateTime.UtcNow);
            await organizationUserRepository.AddAsync(created, cancellationToken);
            await eventPublisher.EnqueueAsync(created.ToJoinedEvent(DateTime.UtcNow), "org.member_joined", cancellationToken);
            return Result<OrganizationMemberDto>.Success(Map(created));
        }

        existing.ChangeRole(request.RoleId);
        await organizationUserRepository.UpdateAsync(existing, cancellationToken);
        return Result<OrganizationMemberDto>.Success(Map(existing));
    }

    public async Task<Result> RemoveMemberAsync(RemoveMemberRequest request, CancellationToken cancellationToken = default)
    {
        var member = await organizationUserRepository.GetByOrgAndUserAsync(request.OrganizationId, request.UserId, cancellationToken);
        if (member is null)
        {
            return Result.Failure(new Error("Org.Member.NotFound", "Member not found."));
        }

        member.Deactivate(DateTime.UtcNow);
        await organizationUserRepository.UpdateAsync(member, cancellationToken);
        await eventPublisher.EnqueueAsync(new OrganizationMemberRemovedEvent(request.OrganizationId, request.UserId, DateTime.UtcNow), "org.member_removed", cancellationToken);
        return Result.Success();
    }

    public async Task<Result> ChangeMemberRoleAsync(ChangeMemberRoleRequest request, CancellationToken cancellationToken = default)
    {
        var member = await organizationUserRepository.GetByOrgAndUserAsync(request.OrganizationId, request.UserId, cancellationToken);
        if (member is null)
        {
            return Result.Failure(new Error("Org.Member.NotFound", "Member not found."));
        }

        member.ChangeRole(request.RoleId);
        await organizationUserRepository.UpdateAsync(member, cancellationToken);
        await eventPublisher.EnqueueAsync(new { org_id = request.OrganizationId, user_id = request.UserId, role_id = request.RoleId }, "org.member_role_changed", cancellationToken);
        return Result.Success();
    }

    public async Task<Result> ChangeMemberActivationAsync(ChangeMemberActivationRequest request, CancellationToken cancellationToken = default)
    {
        var member = await organizationUserRepository.GetByOrgAndUserAsync(request.OrganizationId, request.UserId, cancellationToken);
        if (member is null)
        {
            return Result.Failure(new Error("Org.Member.NotFound", "Member not found."));
        }

        if (request.IsActive)
        {
            member.Activate(DateTime.UtcNow);
        }
        else
        {
            member.Deactivate(DateTime.UtcNow);
        }

        await organizationUserRepository.UpdateAsync(member, cancellationToken);
        await eventPublisher.EnqueueAsync(new { org_id = request.OrganizationId, user_id = request.UserId, active = request.IsActive }, request.IsActive ? "org.member_activated" : "org.member_deactivated", cancellationToken);
        return Result.Success();
    }

    public async Task<Result<bool>> CanMemberAsync(CanMemberRequest request, CancellationToken cancellationToken = default)
    {
        var member = await organizationUserRepository.GetByOrgAndUserAsync(request.OrganizationId, request.UserId, cancellationToken);
        if (member is null)
        {
            return Result<bool>.Failure(new Error("Org.Member.NotFound", "Member not found."));
        }

        // Capability id is resolved by upstream auth/capability catalog.
        if (Guid.TryParse(request.CapabilityKey, out var capabilityId))
        {
            var overridePermission = await organizationUserCapabilityRepository.GetByOrganizationUserAndCapabilityAsync(member.Id, capabilityId, cancellationToken);
            if (overridePermission is not null)
            {
                return Result<bool>.Success(overridePermission.Granted);
            }
        }

        return Result<bool>.Success(true);
    }

    public async Task<Result<InvitationPreviewDto>> SendInvitationAsync(SendInvitationRequest request, CancellationToken cancellationToken = default)
    {
        var invitation = OrganizationUserEntity.CreateInvitation(request.OrganizationId, request.Email, request.RoleId, DateTime.UtcNow, TimeSpan.FromDays(7));
        await organizationUserRepository.AddAsync(invitation, cancellationToken);
        await eventPublisher.EnqueueAsync(
            new OrganizationInvitationSentEvent(request.OrganizationId, request.Email, invitation.InvitationToken!, invitation.InvitationExpiry!.Value, DateTime.UtcNow),
            "org.invitation_sent",
            cancellationToken);
        return Result<InvitationPreviewDto>.Success(MapInvitation(invitation, false));
    }

    public async Task<Result<InvitationPreviewDto>> GetInvitationAsync(string token, CancellationToken cancellationToken = default)
    {
        var invitation = await organizationUserRepository.GetByInvitationTokenAsync(token, cancellationToken);
        if (invitation is null || invitation.InvitationExpiry <= DateTime.UtcNow)
        {
            return Result<InvitationPreviewDto>.Failure(new Error("Org.Invitation.NotFound", "Invitation not found or expired."));
        }

        return Result<InvitationPreviewDto>.Success(MapInvitation(invitation, invitation.UserId != Guid.Empty));
    }

    public async Task<Result> AcceptInvitationAsync(AcceptInvitationRequest request, CancellationToken cancellationToken = default)
    {
        var invitation = await organizationUserRepository.GetByInvitationTokenAsync(request.Token, cancellationToken);
        if (invitation is null || invitation.InvitationExpiry <= DateTime.UtcNow)
        {
            return Result.Failure(new Error("Org.Invitation.NotFound", "Invitation not found or expired."));
        }

        invitation.AcceptInvitation(request.UserId, DateTime.UtcNow);
        await organizationUserRepository.UpdateAsync(invitation, cancellationToken);
        await eventPublisher.EnqueueAsync(invitation.ToJoinedEvent(DateTime.UtcNow), "org.member_joined", cancellationToken);
        return Result.Success();
    }

    public async Task<Result> CancelInvitationAsync(CancelInvitationRequest request, CancellationToken cancellationToken = default)
    {
        var invitation = await organizationUserRepository.GetByIdAsync(request.MembershipId, cancellationToken);
        if (invitation is null)
        {
            return Result.Failure(new Error("Org.Invitation.NotFound", "Invitation not found."));
        }

        invitation.Deactivate(DateTime.UtcNow);
        await organizationUserRepository.UpdateAsync(invitation, cancellationToken);
        return Result.Success();
    }

    public async Task<Result<InvitationPreviewDto>> ResendInvitationAsync(ResendInvitationRequest request, CancellationToken cancellationToken = default)
    {
        var invitation = await organizationUserRepository.GetByIdAsync(request.MembershipId, cancellationToken);
        if (invitation is null || string.IsNullOrWhiteSpace(invitation.InvitationToken))
        {
            return Result<InvitationPreviewDto>.Failure(new Error("Org.Invitation.NotFound", "Invitation not found."));
        }

        await eventPublisher.EnqueueAsync(
            new OrganizationInvitationSentEvent(request.OrganizationId, invitation.InvitationEmail ?? string.Empty, invitation.InvitationToken, invitation.InvitationExpiry ?? DateTime.UtcNow.AddDays(7), DateTime.UtcNow),
            "org.invitation_sent",
            cancellationToken);
        return Result<InvitationPreviewDto>.Success(MapInvitation(invitation, false));
    }

    public Task<Result> UpsertEmailSettingsAsync(UpsertEmailSettingsRequest request, CancellationToken cancellationToken = default)
    {
        return Task.FromResult(Result.Success());
    }

    public Task<Result<QuotaResultDto>> CheckQuotaAsync(CheckQuotaRequest request, CancellationToken cancellationToken = default)
    {
        var quota = request.FeatureKey switch
        {
            "max_properties" => 5,
            "max_units" => 50,
            "max_members" => 10,
            _ => (int?)null
        };

        var allowed = quota is null || request.CurrentUsage < quota;
        var dto = new QuotaResultDto(request.FeatureKey, quota, request.CurrentUsage, allowed, quota is null ? null : Math.Max(0, quota.Value - request.CurrentUsage), allowed ? null : "/settings/subscription/upgrade");
        return Task.FromResult(Result<QuotaResultDto>.Success(dto));
    }

    public async Task<Result<DashboardDto>> GetDashboardAsync(Guid organizationId, CancellationToken cancellationToken = default)
    {
        var orgResult = await GetOrganizationAsync(organizationId, cancellationToken);
        if (orgResult.IsFailure || orgResult.Value is null)
        {
            return Result<DashboardDto>.Failure(orgResult.Error ?? new Error("Org.NotFound", "Organization not found."));
        }

        var members = await GetMembersAsync(organizationId, 1, 10, cancellationToken);
        return Result<DashboardDto>.Success(new DashboardDto(orgResult.Value, members.Value?.Items ?? []));
    }

    private static OrganizationDto Map(OrganizationEntity entity)
        => new(entity.Id, entity.Name, entity.Code, (short)entity.Status, entity.HasEverPaid, entity.Email, entity.Phone, entity.TaxCode, entity.Address);

    private static OrganizationBankingDto Map(OrganizationBankingEntity entity)
        => new(entity.Id, entity.OrganizationId, entity.SepayBankId, entity.AccountNumber, entity.AccountHolderName, entity.BranchName, entity.BranchCode, entity.SwiftCode, entity.IsPrimary);

    private static OrganizationMemberDto Map(OrganizationUserEntity entity)
        => new(entity.Id, entity.OrganizationId, entity.UserId, entity.RoleId, entity.IsActive, entity.CreatedAt, entity.LastActiveAt, entity.LastInactiveAt);

    private static InvitationPreviewDto MapInvitation(OrganizationUserEntity entity, bool hasAccount)
        => new(entity.Id, entity.InvitationEmail ?? string.Empty, entity.InvitationToken ?? string.Empty, entity.InvitationExpiry ?? DateTime.UtcNow, hasAccount);
}
