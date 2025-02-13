namespace Dental_Clinic.Dtos;
using System.ComponentModel.DataAnnotations;
using Dental_Clinic.Enums;

public class UserDto
{
    public int Id { get; set; }

    public int? ClinicId { get; set; }

    public ClinicDto? Clinic { get; set; }

    [Required]
    [EmailAddress]
    [StringLength(100)]
    public required string Email { get; set; }

    [Required]
    [StringLength(100)]
    public required string FirstName { get; init; }

    [Required]
    [StringLength(100)]
    public required string LastName { get; init; }

    [Phone]
    [StringLength(20)]
    public required string Phone { get; set; }

    public UserRole Role { get; set; }

    [StringLength(50)]
    public string? Timezone { get; set; }

    public bool IsActive { get; set; }

    public DateTime CreatedAt { get; init; }

    public DateTime ModifiedAt { get; set; }

    public required string Password { get; set; }

    public ICollection<ServiceDto> Services { get; set; } = new List<ServiceDto>();

    public ICollection<AppointmentDto> Appointments { get; set; } = new List<AppointmentDto>();
    public ReminderType ReminderType { get; set; } = ReminderType.Both;
}
