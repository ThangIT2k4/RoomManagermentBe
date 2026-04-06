using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Property.Application.Services;
using Property.Domain.Repositories;
using Property.Infrastructure.Repositories;
using Property.Infrastructure.Services;
using RoomManagerment.Property.DatabaseSpecific;

namespace Property.Infrastructure;

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
        services.AddScoped<IPropertyRepository, PropertyRepository>();
        return services;
    }
}

