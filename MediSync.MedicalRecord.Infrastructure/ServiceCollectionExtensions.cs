using MediSync.BuildingBlocks.Domain;
using MediSync.MedicalRecord.Application.Abstraction;
using MediSync.MedicalRecord.Domain.Interfaces;
using MediSync.MedicalRecord.Infrastructure.Persistence;
using MediSync.MedicalRecord.Infrastructure.Repositories;
using MediSync.MedicalRecord.Infrastructure.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace MediSync.MedicalRecord.Infrastructure;

public static class ServiceCollectionExtensions
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

        services.AddHttpClient("NotificationApi", client =>
        {
            client.BaseAddress = new Uri(configuration["NotificationApi:BaseUrl"]! ?? "https://localhost:7005");
        });

        services.AddHttpClient("AIApi", client =>
        {
            client.BaseAddress = new Uri(configuration["AIApi:BaseUrl"] ?? "https://localhost:7006");
            client.Timeout = TimeSpan.FromSeconds(30);
        });

        services.AddScoped<INotificationPublisher, NotificationPublisher>();
        services.AddScoped<IAIIndexService, AIIndexService>();

        return services;
    }
}
