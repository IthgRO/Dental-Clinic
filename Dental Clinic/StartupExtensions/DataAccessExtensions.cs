using Microsoft.EntityFrameworkCore;
using Infrastructure;

namespace Dental_Clinic.StartupExtensions
{
    public static class DataAccessExtensions
    {
        public static IServiceCollection AddDataAccess(this IServiceCollection services, string connectionString)
        {
            services.AddDbContext<ApplicationContext>(options => options.UseSqlServer(connectionString));

            return services;
        }
    }
}
