using Dental_Clinic.Enums;
namespace Dental_Clinic.Dtos;

public class ReminderDto
{
    public int Id { get; init; }
    public int AppointmentId { get; init; }
    public ReminderType Type { get; set; }
    public ReminderStatus Status { get; set; }
    public required string Timezone { get; init; }
    public DateTime SendAt { get; set; }
    public DateTime? SentAt { get; set; }
    public DateTime CreatedAt { get; init; }
    public DateTime UpdatedAt { get; set; }
    public AppointmentDto? Appointment { get; init; }
}
