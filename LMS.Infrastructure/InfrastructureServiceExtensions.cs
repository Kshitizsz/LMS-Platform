using Hangfire;
using Hangfire.PostgreSql;
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
        var connectionString = config.GetConnectionString("DefaultConnection");

<<<<<<< HEAD
        // EF Core — SQL Server locally, PostgreSQL in production
        //if (isProduction)
        //{
        services.AddDbContext<AppDbContext>(opt =>
            opt.UseNpgsql(connectionString));
        //}
        //else
        //{
            //services.AddDbContext<AppDbContext>(opt =>
            //    opt.UseSqlServer(connectionString));
        //}
=======
        if (string.IsNullOrWhiteSpace(connectionString))
        {
            throw new InvalidOperationException(
                "Connection string 'DefaultConnection' is not configured. Set ConnectionStrings__DefaultConnection in Render.");
        }
>>>>>>> 590d546a319b2b7df1bab1847a80cad1b66c53f6

        services.AddDbContext<AppDbContext>(opt =>
            opt.UseNpgsql(connectionString));

        services.AddScoped<IUnitOfWork, UnitOfWork>();

        services.AddScoped<IEmailService, EmailService>();
        services.AddScoped<IStripeService, StripeService>();
        services.AddScoped<INotificationService, NotificationService>();
        services.AddScoped<IEmailJobService, EmailJobService>();
        services.AddScoped<IJwtTokenService, JwtTokenService>();

        services.AddHangfire(cfg => cfg
            .SetDataCompatibilityLevel(CompatibilityLevel.Version_180)
            .UseSimpleAssemblyNameTypeSerializer()
            .UseRecommendedSerializerSettings()
            .UsePostgreSqlStorage(options =>
            {
                options.UseNpgsqlConnection(connectionString);
            }));

        services.AddHangfireServer();

        return services;
    }
}
