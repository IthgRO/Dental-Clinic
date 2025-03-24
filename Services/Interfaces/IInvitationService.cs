using Services.Models.Invitation;

namespace Services.Interfaces
{
    public interface IInvitationService
    {
        Task<InvitationDto> SendInvitationToDentist(string email, int adminId);

        Task CancelInvitation(int adminId, int invitationId);

        Task<List<InvitationDto>> GetAdminInvitations(int adminId);
    }
}
