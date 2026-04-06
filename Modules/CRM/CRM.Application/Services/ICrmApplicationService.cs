using CRM.Application.Common;
using CRM.Application.Features.Leads;

namespace CRM.Application.Services;

public interface ICrmApplicationService
{
    Task<Result<LeadDto>> CreateLeadAsync(CreateLeadRequest request, CancellationToken cancellationToken = default);
    Task<Result<LeadDto>> GetLeadByIdAsync(Guid leadId, CancellationToken cancellationToken = default);
    Task<Result<LeadDto>> UpdateLeadStatusAsync(UpdateLeadStatusRequest request, CancellationToken cancellationToken = default);
}
