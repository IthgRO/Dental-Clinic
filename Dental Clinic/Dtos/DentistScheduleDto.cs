using System.ComponentModel.DataAnnotations;
using Dental_Clinic.Enums;
namespace Dental_Clinic.Dtos;

public class DentistScheduleDto
{

    public int Id { get; init; }
    public int DentistId { get; init; }
    public int ClinicId { get; init; }
    public DaysOfWeek DayOfWeek { get; set; }
    public TimeSpan StartTime { get; set; }
    public TimeSpan EndTime { get; set; }
    public required string Timezone { get; set; }
    public bool IsAvailable { get; set; }
    public DateTime CreatedAt { get; init; }
    public DateTime UpdatedAt { get; set; }
}
