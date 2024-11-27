using Dental_Clinic.Enums;
using System.Text.Json.Serialization;

namespace Dental_Clinic.Requests.User
{
    public class RegisterUserRequest
    {
        public int Id { get; set; }

        public int ClinicId { get; set; }

        public required string Email { get; set; }

        public required string FirstName { get; init; }

        public required string LastName { get; init; }

        public required string Phone { get; set; }

        [JsonConverter(typeof(JsonStringEnumConverter<UserRole>))]
        public UserRole Role { get; set; }

        public string? Timezone { get; set; }
    }
}
