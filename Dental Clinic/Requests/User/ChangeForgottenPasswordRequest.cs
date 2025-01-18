namespace Dental_Clinic.Requests.User
{
    public class ChangeForgottenPasswordRequest
    {
        public string Email {  get; set; }

        public string Code {  get; set; }

        public string NewPassword { get; set; }
    }
}
