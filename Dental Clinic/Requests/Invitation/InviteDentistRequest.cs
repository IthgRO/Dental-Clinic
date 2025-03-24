using System.ComponentModel.DataAnnotations;

namespace Dental_Clinic.Requests.Invitation
{
    public class InviteDentistRequest
    {
        [EmailAddress]
        public required string Email {  get; set; }
    }
}
