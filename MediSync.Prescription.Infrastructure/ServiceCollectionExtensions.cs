using MediSync.Prescription.Domain.Interfaces;
using MediSync.Prescription.Infrastructure.Persistence;
using MediSync.Prescription.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace MediSync.Prescription.Infrastructure;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddPrescriptionInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        // Register DbContext
        services.AddDbContext<PrescriptionDbContext>(options =>
        {
            options.UseSqlServer(configuration.GetConnectionString("PrescriptionDatabase"), b => b.MigrationsAssembly("MediSync.Prescription.Infrastructure"));
        });


        // Register repositories
        services.AddScoped<IPrescriptionRepository, PrescriptionRepository>();
        return services;
    }
}
