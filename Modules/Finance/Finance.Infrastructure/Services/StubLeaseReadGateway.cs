using Finance.Application.Services;
using Microsoft.Extensions.Configuration;

namespace Finance.Infrastructure.Services;

public sealed class StubLeaseReadGateway(IConfiguration configuration) : ILeaseReadGateway
{
    public Task<LeaseSummary?> GetLeaseAsync(Guid leaseId, Guid organizationId, CancellationToken cancellationToken = default)
    {
        if (leaseId == Guid.Empty)
        {
            return Task.FromResult<LeaseSummary?>(null);
        }

        if (!configuration.GetValue("Finance:LeaseGateway:UseStub", true))
        {
            return Task.FromResult<LeaseSummary?>(null);
        }

        var status = configuration["Finance:LeaseGateway:Stub:Status"] ?? "active";
        var tenant = configuration.GetValue<Guid?>("Finance:LeaseGateway:Stub:PrimaryResidentUserId");
        return Task.FromResult<LeaseSummary?>(new LeaseSummary(leaseId, status, tenant));
    }

    public Task<IReadOnlyList<Guid>> GetLeaseIdsForResidentUserAsync(
        Guid userId,
        CancellationToken cancellationToken = default)
    {
        var ids = configuration.GetSection("Finance:LeaseGateway:Stub:LeaseIdsForUser").Get<List<Guid>>() ?? [];
        return Task.FromResult<IReadOnlyList<Guid>>(ids);
    }
}
