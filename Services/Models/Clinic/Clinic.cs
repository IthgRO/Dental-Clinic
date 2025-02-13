using Dental_Clinic.Enums;

namespace Services.Models.Clinic
{
    public class Clinic
    {
        public required string Name { get; init; }

        public required string Address { get; init; }

        public required string City { get; init; }

        public required string Country { get; init; }

        public required ClinicTimezone Timezone { get; init; }

        public required string Phone { get; set; }

        public required string Email { get; set; }

        public bool IsActive { get; set; }
    }
}
