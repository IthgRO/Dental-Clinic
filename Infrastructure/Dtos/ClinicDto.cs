namespace Dental_Clinic.Dtos;
using System.ComponentModel.DataAnnotations;
using Dental_Clinic.Enums;

public class ClinicDto
{
    public int Id { get; init; }

    [Required]
    [StringLength(100)]
    public required string Name { get; init; }

    [StringLength(200)]
    public required string Address { get; set; }

    [StringLength(100)]
    public required string City { get; init; }

    [StringLength(100)]
    public required string Country { get; init; }

    [StringLength(50)]
    public required ClinicTimezone Timezone { get; init; }

    [Phone]
    [StringLength(20)]
    public required string Phone { get; set; }

    [EmailAddress]
    [StringLength(100)]
    public required string Email { get; set; }

    public bool IsActive { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set; }

    public byte[]? Picture { get; set; }

    public string? PictureFormat {  get; set; }

    [Required]
    [StringLength(6, MinimumLength = 6)]
    public string ClinicCode { get; init; }

    public ICollection<UserDto> Users { get; set; } = new List<UserDto>();

    public ICollection<AppointmentDto> Appointments { get; set; } = new List<AppointmentDto>();
}
