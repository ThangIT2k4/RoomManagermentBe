using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Organization.Application.Services;
using Organization.Domain.Repositories;
using Organization.Infrastructure.Repositories;
using Organization.Infrastructure.Services;
using RoomManagerment.Organizations.DatabaseSpecific;

namespace Organization.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddScoped<IDataAccessAdapterFactory, DataAccessAdapterFactory>();
        services.AddScoped<DataAccessAdapter>(provider =>
        {
            var factory = provider.GetRequiredService<IDataAccessAdapterFactory>();
            return (DataAccessAdapter)factory.CreateAdapter();
        });

        services.AddScoped<IUnitOfWork, UnitOfWork>();
        services.AddScoped<IOrganizationRepository, OrganizationRepository>();
        return services;
    }
}

