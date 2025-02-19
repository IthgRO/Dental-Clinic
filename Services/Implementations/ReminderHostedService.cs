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

    private DateTime ConvertUtcToLocal(DateTime utcTime, string timezoneId)
    {
        try
        {
            var timeZoneInfo = TimeZoneInfo.FindSystemTimeZoneById(timezoneId);
            return TimeZoneInfo.ConvertTimeFromUtc(utcTime, timeZoneInfo);
        }
        catch (TimeZoneNotFoundException)
        {
            _logger.LogError($"Timezone {timezoneId} not found. Using UTC instead.");
            return utcTime; // Fallback to UTC if timezone is not found
        }
        catch (InvalidTimeZoneException)
        {
            _logger.LogError($"Invalid timezone format: {timezoneId}. Using UTC instead.");
            return utcTime; // Fallback to UTC if the format is invalid
        }
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("ReminderHostedService started.");

        while (!stoppingToken.IsCancellationRequested)
        {
            using var scope = _serviceProvider.CreateScope();
            var reminderService = scope.ServiceProvider.GetRequiredService<IReminderService>();
            var emailService = scope.ServiceProvider.GetRequiredService<IEmailService>();
            var smsService = scope.ServiceProvider.GetRequiredService<ISmsService>();
            var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationContext>();

            var pendingReminders = await reminderService.GetPendingRemindersAsync();

            foreach (var reminder in pendingReminders)
            {
                try
                {
                    if (reminder.Appointment != null && reminder.Appointment.PatientId > 0 && reminder.Appointment != null && reminder.Appointment.Status == AppointmentStatus.Confirmed)
                    {
                        // Convert the appointment's start time from UTC to local time using the appointment's timezone
                        var localStartTime = ConvertUtcToLocal(reminder.Appointment.StartTime, reminder.Appointment.Timezone);

                        // Check the reminder type and send accordingly
                        if (reminder.Type == ReminderType.Email)
                        {
                            var patientEmail = await dbContext.Users
                                .Where(u => u.Id == reminder.Appointment.PatientId)
                                .Select(u => u.Email)
                                .FirstOrDefaultAsync();

                            var email = new EmailDto
                            {
                                To = patientEmail,
                                StartTime = localStartTime,
                                ClinicId = reminder.Appointment.ClinicId,
                                ServiceId = reminder.Appointment.ServiceId,
                                DentistId = reminder.Appointment.DentistId,
                                Status = AppointmentStatus.Pending
                            };

                            emailService.SendReminderEmail(email);
                        }
                        else if (reminder.Type == ReminderType.SMS)
                        {
                            var patientPhone = await dbContext.Users
                                .Where(u => u.Id == reminder.Appointment.PatientId)
                                .Select(u => u.Phone)
                                .FirstOrDefaultAsync();

                            var sms = new SmsDto
                            {
                                To = patientPhone,
                                StartTime = localStartTime,
                                ClinicId = reminder.Appointment.ClinicId,
                                ServiceId = reminder.Appointment.ServiceId,
                                DentistId = reminder.Appointment.DentistId,
                                Status = AppointmentStatus.Pending
                            };

                            smsService.SendSmsReminder(sms);
                        }
                        else if (reminder.Type == ReminderType.Both)
                        {
                            // For Both, send both an email and an SMS reminder.
                            var patientEmail = await dbContext.Users
                                .Where(u => u.Id == reminder.Appointment.PatientId)
                                .Select(u => u.Email)
                                .FirstOrDefaultAsync();
                            var patientPhone = await dbContext.Users
                                .Where(u => u.Id == reminder.Appointment.PatientId)
                                .Select(u => u.Phone)
                                .FirstOrDefaultAsync();

                            var email = new EmailDto
                            {
                                To = patientEmail,
                                StartTime = localStartTime,
                                ClinicId = reminder.Appointment.ClinicId,
                                ServiceId = reminder.Appointment.ServiceId,
                                DentistId = reminder.Appointment.DentistId,
                                Status = AppointmentStatus.Pending
                            };

                            var sms = new SmsDto
                            {
                                To = patientPhone,
                                StartTime = localStartTime,
                                ClinicId = reminder.Appointment.ClinicId,
                                ServiceId = reminder.Appointment.ServiceId,
                                DentistId = reminder.Appointment.DentistId,
                                Status = AppointmentStatus.Pending
                            };

                            emailService.SendReminderEmail(email);
                            smsService.SendSmsReminder(sms);
                        }

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
