namespace Services.Models.User
{
    public class UserUpdateInformationDto
    {
        public required string FirstName { get; set; }
        public required string LastName { get; set; }
        public required string Email { get; set; }
        public required string Phone { get; set; }
    }
}
