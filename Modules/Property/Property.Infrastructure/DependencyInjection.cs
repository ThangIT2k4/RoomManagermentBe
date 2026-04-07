using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Property.Application;
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
        services.AddScoped<IPropertyApplicationService, PropertyApplicationService>();
        services.AddScoped<DataAccessAdapter>(provider =>
        {
            var factory = provider.GetRequiredService<IDataAccessAdapterFactory>();
            return (DataAccessAdapter)factory.CreateAdapter();
        });

        services.AddScoped<IUnitOfWork, UnitOfWork>();
        services.AddScoped<IPropertyRepository, PropertyRepository>();
        services.AddScoped<IUnitRepository, UnitRepository>();
        services.AddScoped<ITicketRepository, TicketRepository>();
        services.AddScoped<IMeterRepository, MeterRepository>();
        services.AddScoped<IMeterReadingRepository, MeterReadingRepository>();
        services.AddScoped<IAmenityRepository, AmenityRepository>();
        services.AddScoped<IPropertyTypeRepository, PropertyTypeRepository>();
        services.AddScoped<IPropertyStaffRepository, PropertyStaffRepository>();
        services.AddScoped<IVendorRepository, VendorRepository>();
        services.AddScoped<IDocumentRepository, DocumentRepository>();
        return services;
    }
}

