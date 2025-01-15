namespace Dental_Clinic.Requests.Appointment
{
    public class UpdateAppointmentRequest
    {
        public int AppointmentId {  get; set; }
        public DateTime NewDate { get; set; }
    }
}
