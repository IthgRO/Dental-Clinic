namespace Dental_Clinic.Requests.Service
{
    public class AddServiceToDentistRequest
    {
        public required string Name {  get; set; }
        public required string Category { get; set; }
        public int Duration {  get; set; }
    }
}
