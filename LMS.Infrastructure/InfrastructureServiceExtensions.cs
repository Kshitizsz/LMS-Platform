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
        var connectionString = config.GetConnectionString("DefaultConnection")!;
        var isProduction = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT")
                           == "Production";

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

        // Unit of Work
        services.AddScoped<IUnitOfWork, UnitOfWork>();

        // App Services
        services.AddScoped<IEmailService, EmailService>();
        services.AddScoped<IStripeService, StripeService>();
        services.AddScoped<INotificationService, NotificationService>();
        services.AddScoped<IEmailJobService, EmailJobService>();
        services.AddScoped<IJwtTokenService, JwtTokenService>();

        // Hangfire
        if (isProduction)
        {
            services.AddHangfire(cfg => cfg
  .SetDataCompatibilityLevel(CompatibilityLevel.Version_180)
  .UseSimpleAssemblyNameTypeSerializer()
  .UseRecommendedSerializerSettings()
  .UsePostgreSqlStorage(options =>
  {
      options.UseNpgsqlConnection(connectionString);
  })
);
        }
        else
        {
            services.AddHangfire(cfg => cfg
                .SetDataCompatibilityLevel(CompatibilityLevel.Version_180)
                .UseSimpleAssemblyNameTypeSerializer()
                .UseRecommendedSerializerSettings()

            .UsePostgreSqlStorage(options =>
            {
                options.UseNpgsqlConnection(connectionString);
            })
            );
            //.UseSqlServerStorage(connectionString));
        }

        services.AddHangfireServer();

        return services;
    }
}














//using Hangfire;
//using Hangfire.SqlServer;
//using Hangfire.PostgreSql;
//using LMS.Domain.Interfaces;
//using LMS.Infrastructure.Persistence;
//using LMS.Infrastructure.Repository;
//using LMS.Infrastructure.Services;
//using Microsoft.EntityFrameworkCore;
//using Microsoft.Extensions.Configuration;
//using Microsoft.Extensions.DependencyInjection;

//namespace LMS.Infrastructure;

//public static class InfrastructureServiceExtensions
//{
//    public static IServiceCollection AddInfrastructure(
//        this IServiceCollection services,
//        IConfiguration config)

//    {
//        var connectionString = config.GetConnectionString("DefaultConnection")!;
//        var isProduction = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT")
//                           == "Production";

//        // EF Core — SQL Server locally, PostgreSQL in production
//        if (isProduction)
//        {
//            services.AddDbContext<AppDbContext>(opt =>
//                opt.UseNpgsql(connectionString));
//        }
//        else
//        {
//            services.AddDbContext<AppDbContext>(opt =>
//                opt.UseSqlServer(connectionString));
//        }
//        {
//            // EF Core
//            services.AddDbContext<AppDbContext>(opt =>
//                opt.UseSqlServer(config.GetConnectionString("DefaultConnection")));

//            // Add inside AddInfrastructure method
//            services.AddScoped<INotificationService, NotificationService>();
//            services.AddScoped<IEmailJobService, EmailJobService>();
//            // Unit of Work
//            services.AddScoped<IUnitOfWork, UnitOfWork>();

//            // App Services
//            services.AddScoped<IEmailService, EmailService>();
//            services.AddScoped<IStripeService, StripeService>();

//            // Hangfire
//            services.AddHangfire(cfg => cfg
//                .SetDataCompatibilityLevel(CompatibilityLevel.Version_180)
//                .UseSimpleAssemblyNameTypeSerializer()
//                .UseRecommendedSerializerSettings()
//                .UseSqlServerStorage(config.GetConnectionString("DefaultConnection")));
//            services.AddHangfireServer();

//            return services;
//        }
//    }
//}