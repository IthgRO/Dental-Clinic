using Dental_Clinic.Enums;
namespace Dental_Clinic.Dtos;

public class AppointmentDto
{
    public int Id { get; init; }
    public int ClinicId { get; init; }
    public int ServiceId { get; init; }
    public int PatientId { get; init; }
    public int DentistId { get; init; }
    public ClinicDto? Clinic { get; init; }
    public UserDto? Dentist { get; init; }
    public ServiceDto? Service { get; init; }
    public DateTime StartTime { get; set; }
    public DateTime EndTime { get; set; }
    public required string Timezone { get; set; }
    public AppointmentStatus Status { get; set; }
    public string? Notes { get; set; }
    public DateTime CreatedAt { get; init; }
    public DateTime UpdatedAt { get; set; }

}
