namespace Dental_Clinic.Requests.Dentist
{
    public class SendLoginCodeRequest
    {
        public required string Email {  get; set; }
        public required string Password { get; set; }
    }
}
