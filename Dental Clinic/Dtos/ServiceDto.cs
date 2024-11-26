using Dental_Clinic.Enums;
namespace Dental_Clinic.Dtos;

public class ServiceDto
{
    public int Id { get; init; }
    public int ClinicId { get; init; }
    public int? DentistId { get; init; }
    public required string Name { get; init; }
    public string? Description { get; set; }
    public int Duration { get; set; }
    public decimal Price { get; set; }
    public required string Category { get; set; }
    public bool IsActive { get; set; }
    public DateTime CreatedAt { get; init; }
    public DateTime UpdatedAt { get; set; }
    }
