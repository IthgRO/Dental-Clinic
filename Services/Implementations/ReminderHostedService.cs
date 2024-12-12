using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Services.Interfaces;
using Services.Models;
using Microsoft.Extensions.Hosting;
using Infrastructure;
using Microsoft.EntityFrameworkCore;
using Dental_Clinic.Enums;

public class ReminderHostedService : BackgroundService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<ReminderHostedService> _logger;

    public ReminderHostedService(IServiceProvider serviceProvider, ILogger<ReminderHostedService> logger)
    {
        _serviceProvider = serviceProvider;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("ReminderHostedService started.");

        while (!stoppingToken.IsCancellationRequested)
        {
            using var scope = _serviceProvider.CreateScope();
            var reminderService = scope.ServiceProvider.GetRequiredService<IReminderService>();
            var emailService = scope.ServiceProvider.GetRequiredService<IEmailService>();
            var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationContext>();

            var pendingReminders = await reminderService.GetPendingRemindersAsync();

            foreach (var reminder in pendingReminders)
            {
                try
                {
                    if (reminder.Appointment != null && reminder.Appointment.PatientId > 0)
                    {
                        var patientEmail = await dbContext.Users
                            .Where(u => u.Id == reminder.Appointment.PatientId)
                            .Select(u => u.Email)
                            .FirstOrDefaultAsync();
                        var email = new EmailDto
                        {
                            To = patientEmail,
                            StartTime = reminder.Appointment.StartTime,
                            ClinicId = reminder.Appointment.ClinicId,
                            ServiceId = reminder.Appointment.ServiceId,
                            DentistId = reminder.Appointment.DentistId,
                            Status = AppointmentStatus.Pending
        
                        };

                        emailService.SendEmail(email);

                        await reminderService.MarkReminderAsSentAsync(reminder.Id);
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, $"Error processing reminder ID {reminder.Id}");
                }
            }

            await Task.Delay(TimeSpan.FromMinutes(5), stoppingToken);
        }
    }
}

