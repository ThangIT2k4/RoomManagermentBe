using CRM.Application.Common;
using CRM.Application.Features.Leads;
using CRM.Application.Services;
using CRM.Domain.Entities;
using CRM.Domain.Repositories;

namespace CRM.Infrastructure.Services;

public sealed class CrmApplicationService(ILeadRepository leadRepository) : ICrmApplicationService
{
    public async Task<Result<LeadDto>> CreateLeadAsync(CreateLeadRequest request, CancellationToken cancellationToken = default)
    {
        if (request.OrganizationId == Guid.Empty)
        {
            return Result<LeadDto>.Failure(new Error("CRM.Lead.Organization.Required", "OrganizationId is required."));
        }

        LeadEntity entity;
        try
        {
            entity = LeadEntity.Create(request.OrganizationId, request.FullName, request.Status);
        }
        catch (ArgumentException ex)
        {
            return Result<LeadDto>.Failure(new Error("CRM.Lead.InvalidInput", ex.Message));
        }

        var created = await leadRepository.AddAsync(entity, cancellationToken);
        return Result<LeadDto>.Success(Map(created));
    }

    public async Task<Result<LeadDto>> GetLeadByIdAsync(Guid leadId, CancellationToken cancellationToken = default)
    {
        if (leadId == Guid.Empty)
        {
            return Result<LeadDto>.Failure(new Error("CRM.Lead.Id.Required", "LeadId is required."));
        }

        var lead = await leadRepository.GetByIdAsync(leadId, cancellationToken);
        if (lead is null)
        {
            return Result<LeadDto>.Failure(new Error("CRM.Lead.NotFound", "Lead not found."));
        }

        return Result<LeadDto>.Success(Map(lead));
    }

    public async Task<Result<LeadDto>> UpdateLeadStatusAsync(UpdateLeadStatusRequest request, CancellationToken cancellationToken = default)
    {
        if (request.LeadId == Guid.Empty)
        {
            return Result<LeadDto>.Failure(new Error("CRM.Lead.Id.Required", "LeadId is required."));
        }

        var lead = await leadRepository.GetByIdAsync(request.LeadId, cancellationToken);
        if (lead is null)
        {
            return Result<LeadDto>.Failure(new Error("CRM.Lead.NotFound", "Lead not found."));
        }

        try
        {
            lead.UpdateStatus(request.Status);
        }
        catch (ArgumentException ex)
        {
            return Result<LeadDto>.Failure(new Error("CRM.Lead.InvalidStatus", ex.Message));
        }

        var updated = await leadRepository.UpdateAsync(lead, cancellationToken);
        return Result<LeadDto>.Success(Map(updated));
    }

    private static LeadDto Map(LeadEntity lead)
    {
        return new LeadDto(
            lead.Id,
            lead.OrganizationId,
            lead.FullName,
            lead.Status,
            lead.CreatedAt,
            lead.UpdatedAt);
    }
}
