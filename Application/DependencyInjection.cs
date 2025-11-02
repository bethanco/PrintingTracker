using Microsoft.Extensions.DependencyInjection;
using PrintingJobTracker.Services;

namespace PrintingJobTracker.Application
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddApplication(this IServiceCollection services)
        {
            services.AddScoped<IWorkOrdersService, WorkOrdersService>();
            return services;
        }
    }
}
