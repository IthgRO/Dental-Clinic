namespace Dental_Clinic.Dtos;

public class DentistScheduleDto
{

    public int Id { get; init; }
    public int DentistId { get; init; }
    public int ClinicId { get; init; }
    public DayOfWeek DayOfWeek { get; set; }
    public TimeSpan StartTime { get; set; }
    public TimeSpan EndTime { get; set; }
}
