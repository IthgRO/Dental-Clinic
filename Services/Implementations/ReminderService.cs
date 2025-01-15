using Dental_Clinic.Dtos;
using Infrastructure;
using Services.Interfaces;
using Dental_Clinic.Enums;
using Microsoft.EntityFrameworkCore;

public class ReminderService : IReminderService
{
    private readonly ApplicationContext _db;

    public ReminderService(ApplicationContext db)
    {
        _db = db;
    }

    public async Task CreateReminderAsync(ReminderDto reminder)
    {
        var newReminder = new ReminderDto
        {
            AppointmentId = reminder.AppointmentId,
            Type = reminder.Type,
            Status = reminder.Status,
            Timezone = reminder.Timezone,
            SendAt = reminder.SendAt,
            CreatedAt = reminder.CreatedAt,
            UpdatedAt = reminder.UpdatedAt
        };

        _db.Reminders.Add(newReminder);
        await _db.SaveChangesAsync();
    }

    public async Task<IEnumerable<ReminderDto>> GetPendingRemindersAsync()
    {
        return await _db.Reminders
            .Where(r => r.Status == ReminderStatus.Pending && r.SendAt <= DateTime.UtcNow.AddHours(2))
            .Select(r => new ReminderDto
            {
                Id = r.Id,
                AppointmentId = r.AppointmentId,
                Type = r.Type,
                Status = r.Status,
                Timezone = r.Timezone,
                SendAt = r.SendAt,
                SentAt = r.SentAt,
                CreatedAt = r.CreatedAt,
                UpdatedAt = r.UpdatedAt,
                Appointment = r.Appointment != null ? new AppointmentDto
                {
                    Id = r.Appointment.Id,
                    StartTime = r.Appointment.StartTime,
                    EndTime = r.Appointment.EndTime,
                    ClinicId = r.Appointment.ClinicId,
                    DentistId = r.Appointment.DentistId,
                    PatientId = r.Appointment.PatientId,
                    ServiceId = r.Appointment.ServiceId,
                    Timezone = r.Appointment.Timezone
                } : null
            })
            .ToListAsync();
    }

    public async Task MarkReminderAsSentAsync(int reminderId)
    {
        var reminder = await _db.Reminders.FindAsync(reminderId);
        if (reminder != null)
        {
            reminder.Status = ReminderStatus.Sent;
            reminder.SentAt = DateTime.UtcNow;
            await _db.SaveChangesAsync();
        }
    }
}
