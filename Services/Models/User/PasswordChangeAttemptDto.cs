namespace Services.Models.User
{
    public class PasswordChangeAttemptDto
    {
        public string Email {  get; set; }

        public string Code {  get; set; }

        public string FirstName {  get; set; }
    }
}
