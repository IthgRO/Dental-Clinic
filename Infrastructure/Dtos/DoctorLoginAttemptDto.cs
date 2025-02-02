namespace Infrastructure.Dtos
{
    public class DoctorLoginAttemptDto
    {
        public int Id { get; set; }
        public required string Email {  get; set; }
        public required string Code { get; set; }
        public DateTime DateCreated { get; set; }
    }
}
