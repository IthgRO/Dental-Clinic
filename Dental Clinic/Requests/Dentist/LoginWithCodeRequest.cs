namespace Dental_Clinic.Requests.Dentist
{
    public class LoginWithCodeRequest
    {
        public required string Email { get; set; }
        public required string Password { get; set; }
        public required string Code { get; set; }
    }
}
