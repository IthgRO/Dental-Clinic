using Dental_Clinic.Enums;

namespace Dental_Clinic.Responses.Clinic
{
    public class DentistClinicViewModel
    {
        public required string Name { get; init; }

        public required string Address { get; init; }

        public required string City { get; init; }

        public required string Country { get; init; }

        public required ClinicTimezone Timezone { get; init; }

        public required string Phone { get; set; }

        public required string Email { get; set; }
    }
}
