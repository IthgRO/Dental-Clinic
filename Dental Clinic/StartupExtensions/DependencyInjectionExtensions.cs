using Dental_Clinic.Requests.Service;
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
            services.AddScoped<IEmailService, EmailService>();
            services.AddScoped<IClinicService, ClinicService>();
            services.AddScoped<IReminderService, ReminderService>();
            services.AddScoped<IDentistService, DentistService>();
            services.AddScoped<IServiceService, ServiceService>();
            services.AddScoped<IPasswordService, PasswordService>();
            services.AddScoped<IScheduleService, ScheduleService>();
            services.AddScoped<IValidator<RegisterUserRequest>, RegisterUserRequestAbstractValidator>();
            services.AddScoped<IValidator<AddServiceToDentistRequest>, AddServiceToDentistRequestAbstractValidator>();

            return services;
        }
    }
}
