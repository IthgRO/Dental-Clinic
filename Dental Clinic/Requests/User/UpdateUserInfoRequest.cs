namespace Dental_Clinic.Requests.User
{
    public class UpdateUserInfoRequest
    {
        public required string Email {  get; set; }
        public required string Phone { get; set; }
    }
}
