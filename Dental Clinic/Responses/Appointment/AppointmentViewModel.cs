namespace Dental_Clinic.Responses.Appointment
{
    public class AppointmentViewModel
    {
        public int Id { get; set; }
        public required string DentistFirstName { get; set; }
        public required string DentistLastName { get; set; }
        public required string ClinicName {  get; set; }
        public required string City {  get; set; }
        public required string ServiceName {  get; set; }
        public decimal ServicePrice {  get; set; }
        public required string Currency {  get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
    }
}
