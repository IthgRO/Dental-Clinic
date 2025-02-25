using System.ComponentModel.DataAnnotations;

namespace Dental_Clinic.Requests.User
{
    public class RegisterDentistRequest
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        public string FirstName { get; init; }

        [Required]
        public string LastName { get; init; }

        [Required]
        [Phone]
        public string Phone { get; set; }

        [Required]
        public string Password { get; set; }

        // Instead of ClinicId, dentists now provide a 6-digit clinic code.
        [Required]
        [StringLength(6, MinimumLength = 6)]
        public string ClinicCode { get; set; }

        // Optionally, you can include a timezone override.
        public string? Timezone { get; set; }
    }
}
