namespace Services.Models.Reservation
{
    public class AppointmentDto
    {
        public int Id { get; set; }
        public int DentistId {  get; set; }
        public required string DentistFirstName { get; set; }
        public required string DentistLastName { get; set; }
        public required string ClinicName { get; set; }
        public required string City { get; set; }
        public required string ServiceName { get; set; }
        public decimal ServicePrice { get; set; }
        public required string Currency { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
    }
}
