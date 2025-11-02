using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using PrintingJobTracker.Data;
using PrintingJobTracker.Services;

namespace PrintingJobTracker.Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
            var connectionString = configuration.GetConnectionString("DefaultConnection")
                ?? "Server=(localdb)\\MSSQLLocalDB;Database=PrintingJobTrackerDb;Trusted_Connection=True;MultipleActiveResultSets=true;TrustServerCertificate=True";

            services.AddDbContext<OpsDbContext>(options =>
                options.UseSqlServer(connectionString));

            services.AddScoped<DataSeeder>();
            services.AddScoped<PrintingJobTracker.Infrastructure.Seed.DataInitializer>();
            return services;
        }
    }
}
