namespace Services.Models.User
{
    public class LoginResultDto
    {
        public required string Jwt {  get; set; }
        public required string FirstName { get; set; }
        public required string LastName { get; set; }
        public required string Email { get; set; }
        public required string Phone { get; set; }
        public int Role {  get; set; }
    }
}
