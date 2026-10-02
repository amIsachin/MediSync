using MediSync.BuildingBlocks.Domain;
using MediSync.Prescription.Application.Abstractions;
using MediSync.Prescription.Domain.Interfaces;
using MediSync.Prescription.Infrastructure.Persistence;
using MediSync.Prescription.Infrastructure.Repositories;
using MediSync.Prescription.Infrastructure.Services;
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

        services.AddHttpClient("NotificationApi", client =>
        {
            client.BaseAddress = new Uri(configuration["NotificationApi:BaseUrl"]! ?? "https://localhost:7005");
        });

        services.AddHttpClient("AIApi", client =>
        {
            client.BaseAddress = new Uri(configuration["AI:BaseUrl"]! ?? "https://localhost:7006");
            client.Timeout = TimeSpan.FromMinutes(10);
        });

        services.AddHttpClient("Gateway", client =>
        {
            client.BaseAddress = new Uri(configuration["Gateway:BaseUrl"]! ?? "https://localhost:7000");
            client.Timeout = TimeSpan.FromMinutes(10);
        });

        services.AddHttpClient("AIApi", client =>
        {
            client.BaseAddress = new Uri(configuration["AIApi:BaseUrl"] ?? "https://localhost:7006");
            client.Timeout = TimeSpan.FromSeconds(30);
        });

        services.AddScoped<INotificationPublisher, NotificationPublisher>();
        services.AddScoped<IAIService, AIService>();
        services.AddScoped<IAIInteractionChecker, AIInteractionChecker>();
        services.AddScoped<IPatientDataService, PatientDataService>();
        services.AddScoped<IAIPrescriptionIndexService, AIPrescriptionIndexService>();

        return services;
    }
}
