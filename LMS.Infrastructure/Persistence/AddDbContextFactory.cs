using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using System.IO;

namespace LMS.Infrastructure.Persistence
{
    public class AppDbContextFactory : IDesignTimeDbContextFactory<AppDbContext>
    {
        public AppDbContext CreateDbContext(string[] args)
        {
            var basePath = Directory.GetCurrentDirectory();

            var config = new ConfigurationBuilder()
                .SetBasePath(Path.Combine(basePath, "../LMS.API"))
                .AddJsonFile("appsettings.json", optional: false)
                //.AddJsonFile("appsettings.Development.json", optional: false)
                //.AddJsonFile("appsettings.Production.json", optional: true)
                .AddEnvironmentVariables()
                .Build();

            var connectionString = config.GetConnectionString("DefaultConnection");

            var optionsBuilder = new DbContextOptionsBuilder<AppDbContext>();
            optionsBuilder.UseNpgsql(connectionString);

            return new AppDbContext(optionsBuilder.Options);
        }
    }
}