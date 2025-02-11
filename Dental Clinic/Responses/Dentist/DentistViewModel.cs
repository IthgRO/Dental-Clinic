using Dental_Clinic.Responses.Clinic;
using Dental_Clinic.Responses.Service;

namespace Dental_Clinic.Responses.Dentist
{
    public class DentistViewModel
    {
        public int Id { get; set; }

        public required string FirstName { get; set; }

        public required string LastName { get; set; }

        public required string Email {  get; set; }

        public required string Phone { get; set; }

        public int MaximumPrice { get; set; }

        public int MinimumPrice { get; set; }

        public ClinicViewModel? Clinic { get; set; }

        public List<ServiceViewModel>? Services { get; set; } = [];
    }
}
