namespace Dental_Clinic.Requests.Appointment
{
    public class BookAppointmentRequest
    {
        public int DentistId {  get; set; }

        public int ClinicId {  get; set; }

        public int ServiceId {  get; set; }

        public DateTime StartDate { get; set; }
    }
}
