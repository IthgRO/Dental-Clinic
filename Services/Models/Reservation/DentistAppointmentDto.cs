using Dental_Clinic.Enums;

namespace Services.Models.Reservation
{
    public class DentistAppointmentDto
    {
        public int Id { get; set; }
        public required string FirstName { get; set; }
        public required string LastName { get; set; }
        public required string ServiceName { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public AppointmentStatus Status { get; set; }
        public ClinicTimezone TimeZone { get; set; }
    }
}
