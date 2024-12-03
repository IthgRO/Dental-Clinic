namespace Dental_Clinic.Requests.Appointment
{
    public class GetFreeSlotsRequest
    {
        public int DentistId {  get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
    }
}
