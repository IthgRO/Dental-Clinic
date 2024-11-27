using Dental_Clinic.Enums;
namespace Dental_Clinic.Dtos;

public class ReminderSettingsDto
{
        public int Id { get; init; }
        public int UserId { get; init; }
        public int ClinicId { get; init; }
        public ReminderType Type { get; set; }
        public TimeSpan TimeBefore { get; init; }
        public DateTime CreatedAt { get; init; }
        public DateTime UpdatedAt { get; set; }
    }
