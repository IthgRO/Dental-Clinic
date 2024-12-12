using System;
using Dental_Clinic.Dtos;

namespace Services.Interfaces;

public interface IReminderService
{
    Task CreateReminderAsync(ReminderDto reminder);
    Task<IEnumerable<ReminderDto>> GetPendingRemindersAsync();
    Task MarkReminderAsSentAsync(int reminderId);
}
