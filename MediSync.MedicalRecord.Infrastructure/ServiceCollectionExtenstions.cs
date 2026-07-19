using MediSync.MedicalRecord.Domain.Interfaces;
using MediSync.MedicalRecord.Infrastructure.Persistence;
using MediSync.MedicalRecord.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace MediSync.MedicalRecord.Infrastructure;

public static class ServiceCollectionExtenstions
{
    public static IServiceCollection AddMedicalRecordInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<MedicalRecordDbContext>(options =>
        {
            // Configure your database provider and connection string here
            options.UseSqlServer(configuration.GetConnectionString("DefaultConnection"),
                b => b.MigrationsAssembly("MediSync.MedicalRecord.Infrastructure"));
        });

        // Register repositories
        services.AddScoped<IPatientRepository, PatientRepository>();

        return services;
    }
}
