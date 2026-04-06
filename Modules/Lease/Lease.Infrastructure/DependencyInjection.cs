using Lease.Application.Services;
using Lease.Domain.Repositories;
using Lease.Infrastructure.Repositories;
using Lease.Infrastructure.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using RoomManagerment.Lease.DatabaseSpecific;

namespace Lease.Infrastructure;

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
        services.AddScoped<ILeaseRepository, LeaseRepository>();
        return services;
    }
}

