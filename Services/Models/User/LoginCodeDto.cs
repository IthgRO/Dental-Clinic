namespace Services.Models.User
{
    public class LoginCodeDto
    {
        public required string Email {  get; set; }
        public required string FirstName {  get; set; }
        public required string Code { get; set; }
    }
}
