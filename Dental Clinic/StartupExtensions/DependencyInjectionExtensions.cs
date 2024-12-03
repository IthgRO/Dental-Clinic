using Dental_Clinic.Requests.User;
using FluentValidation;
using Services.Implementations;
using Services.Interfaces;

namespace Dental_Clinic.StartupExtensions
{
    public static class DependencyInjectionExtensions
    {
        public static IServiceCollection AddDependencyInjection(this IServiceCollection services)
        {
            services.AddScoped<IUserService, UserService>();
            services.AddScoped<IAppointmentService, AppointmentService>();

            services.AddScoped<IValidator<RegisterUserRequest>, RegisterUserRequestAbstractValidator>();

            return services;
        }
    }
}
