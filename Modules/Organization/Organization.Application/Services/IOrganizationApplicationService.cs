using Organization.Application.Common;
using Organization.Application.Dtos;

namespace Organization.Application.Services;

public interface IOrganizationApplicationService
{
    Task<Result<OrganizationDto>> GetOrganizationAsync(Guid organizationId, CancellationToken cancellationToken = default);
    Task<Result<OrganizationDto>> OnboardOrganizationAsync(OnboardOrganizationRequest request, CancellationToken cancellationToken = default);
    Task<Result<OrganizationDto>> UpdateOrganizationAsync(UpdateOrganizationRequest request, CancellationToken cancellationToken = default);

    Task<Result<OrganizationBankingDto>> AddBankingAsync(AddOrganizationBankingRequest request, CancellationToken cancellationToken = default);
    Task<Result> SetPrimaryBankingAsync(SetPrimaryBankingRequest request, CancellationToken cancellationToken = default);
    Task<Result> RemoveBankingAsync(RemoveBankingRequest request, CancellationToken cancellationToken = default);

    Task<Result<PagedResponse<OrganizationMemberDto>>> GetMembersAsync(Guid organizationId, int page, int pageSize, CancellationToken cancellationToken = default);
    Task<Result<OrganizationMemberDto>> UpsertMemberAsync(UpsertMemberRequest request, CancellationToken cancellationToken = default);
    Task<Result> RemoveMemberAsync(RemoveMemberRequest request, CancellationToken cancellationToken = default);
    Task<Result> ChangeMemberRoleAsync(ChangeMemberRoleRequest request, CancellationToken cancellationToken = default);
    Task<Result> ChangeMemberActivationAsync(ChangeMemberActivationRequest request, CancellationToken cancellationToken = default);
    Task<Result<bool>> CanMemberAsync(CanMemberRequest request, CancellationToken cancellationToken = default);

    Task<Result<InvitationPreviewDto>> SendInvitationAsync(SendInvitationRequest request, CancellationToken cancellationToken = default);
    Task<Result<InvitationPreviewDto>> GetInvitationAsync(string token, CancellationToken cancellationToken = default);
    Task<Result> AcceptInvitationAsync(AcceptInvitationRequest request, CancellationToken cancellationToken = default);
    Task<Result> CancelInvitationAsync(CancelInvitationRequest request, CancellationToken cancellationToken = default);
    Task<Result<InvitationPreviewDto>> ResendInvitationAsync(ResendInvitationRequest request, CancellationToken cancellationToken = default);

    Task<Result> UpsertEmailSettingsAsync(UpsertEmailSettingsRequest request, CancellationToken cancellationToken = default);
    Task<Result<QuotaResultDto>> CheckQuotaAsync(CheckQuotaRequest request, CancellationToken cancellationToken = default);
    Task<Result<DashboardDto>> GetDashboardAsync(Guid organizationId, CancellationToken cancellationToken = default);
}
