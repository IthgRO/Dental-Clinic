using Dental_Clinic.Enums;
namespace Dental_Clinic.Dtos;
using System.Text.Json;

public class AppointmentChangeDto
{
     public int Id { get; init; }
    public int AppointmentId { get; init; }
    public int ChangedById { get; init; }
    public AppointmentChangeType ChangeType { get; init; }
    public string? OldData { get; set; }
    public string? NewData { get; set; }
    public required string Timezone { get; init; }
    public DateTime CreatedAt { get; init; }
    
    // Deserialzation of JSON data
    public T? GetOldData<T>() where T : class
            => string.IsNullOrEmpty(OldData) 
                ? null 
                : JsonSerializer.Deserialize<T>(OldData);

    public T? GetNewData<T>() where T : class
        => string.IsNullOrEmpty(NewData) 
            ? null 
            : JsonSerializer.Deserialize<T>(NewData);
}

