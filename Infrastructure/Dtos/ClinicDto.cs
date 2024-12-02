namespace Dental_Clinic.Dtos;
using System.ComponentModel.DataAnnotations;
public class ClinicDto
{
    public int Id { get; init; }

    [Required]
    [StringLength(100)]
    public required string Name { get; init; }

    [StringLength(200)]
    public required string Address { get; init; }

    [StringLength(100)]
    public required string City { get; init; }

    [StringLength(100)]
    public required string Country { get; init; }

    [StringLength(50)]
    public required string Timezone { get; init; }

    [Phone]
    [StringLength(20)]
    public required string Phone { get; set; }

    [EmailAddress]
    [StringLength(100)]
    public required string Email { get; set; }

    public bool IsActive { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set; }

    public ICollection<UserDto> Users { get; set; } = new List<UserDto>();
}
