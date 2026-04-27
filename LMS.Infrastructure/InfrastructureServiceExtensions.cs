using Hangfire;
using Hangfire.SqlServer;
using LMS.Domain.Interfaces;
using LMS.Infrastructure.Persistence;
using LMS.Infrastructure.Repository;
using LMS.Infrastructure.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace LMS.Infrastructure;

public static class InfrastructureServiceExtensions
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration config)
    {
        // EF Core
        services.AddDbContext<AppDbContext>(opt =>
            opt.UseSqlServer(config.GetConnectionString("DefaultConnection")));

        // Add inside AddInfrastructure method
        services.AddScoped<INotificationService, NotificationService>();
        services.AddScoped<IEmailJobService, EmailJobService>();
        // Unit of Work
        services.AddScoped<IUnitOfWork, UnitOfWork>();

        // App Services
        services.AddScoped<IEmailService, EmailService>();
        services.AddScoped<IStripeService, StripeService>();

        // Hangfire
        services.AddHangfire(cfg => cfg
            .SetDataCompatibilityLevel(CompatibilityLevel.Version_180)
            .UseSimpleAssemblyNameTypeSerializer()
            .UseRecommendedSerializerSettings()
            .UseSqlServerStorage(config.GetConnectionString("DefaultConnection")));
        services.AddHangfireServer();

        return services;
    }
}