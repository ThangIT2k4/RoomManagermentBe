using Integration.Domain.Enums;
using Integration.Domain.Providers;

namespace Integration.Application.Services;

public interface IIntegrationProviderClientFactory
{
    IIntegrationProviderClient GetClient(IntegrationProvider provider);
}
