using Dental_Clinic.Enums;

namespace Dental_Clinic.Responses.Invitation
{
    public class InvitationViewModel
    {
        public int Id { get; set; }
        public int AdminId { get; set; }
        public required string DentistEmail { get; set; }
        public InvitationStatus Status { get; set; }
    }
}
