using Dental_Clinic.Enums;
namespace Dental_Clinic.Dtos;

public class ServiceDto
{
    public int Id { get; init; }
    public int? UserId { get; init; }
    public UserDto? User { get; init; }
    public required string Name { get; init; }
    public string? Description { get; set; }
    public int Duration { get; set; }
    public decimal Price { get; set; }
    public required string Currency {  get; set; }
    public required string Category { get; set; }
    public bool IsActive { get; set; }
    public DateTime CreatedAt { get; init; }
    public DateTime UpdatedAt { get; set; }
}
