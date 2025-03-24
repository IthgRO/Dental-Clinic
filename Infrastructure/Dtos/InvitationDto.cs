using Dental_Clinic.Enums;

namespace Infrastructure.Dtos
{
    public class InvitationDto
    {
        public int Id { get; set; }
        public int AdminId {  get; set; }
        public required string DentistEmail {  get; set; }
        public InvitationStatus Status { get; set; }
    }
}
