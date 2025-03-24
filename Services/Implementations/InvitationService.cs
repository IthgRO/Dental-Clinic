using Infrastructure;
using Infrastructure.Dtos;
using Microsoft.EntityFrameworkCore;
using Services.Interfaces;
using Services.Models.Invitation;

namespace Services.Implementations
{
    public class InvitationService : IInvitationService
    {
        private readonly ApplicationContext _db;

        public InvitationService(ApplicationContext db) 
        { 
            _db = db;
        }

        public async Task CancelInvitation(int adminId, int invitationId)
        {
            var admin = await _db.Users
                .Where(x => x.Id == adminId && x.Role == Dental_Clinic.Enums.UserRole.Admin)
                .FirstOrDefaultAsync() ?? throw new Exception("User does not exist or is not an admin!");

            var existingInvitation = await _db.Invitations
                .Where(x => x.AdminId == adminId && x.Id == invitationId)
                .FirstOrDefaultAsync() ?? throw new Exception("Invitation could not be found or does not belong to this admin!");

            if(existingInvitation.Status == Dental_Clinic.Enums.InvitationStatus.Accepted)
            {
                throw new Exception("The invitation was already accepted!");
            }

            _db.Invitations.Remove(existingInvitation);
            await _db.SaveChangesAsync();
        }

        public async Task<List<Models.Invitation.InvitationDto>> GetAdminInvitations(int adminId)
        {
            var admin = await _db.Users
                .Where(x => x.Id == adminId && x.Role == Dental_Clinic.Enums.UserRole.Admin)
                .Include(x => x.Clinic)
                .FirstOrDefaultAsync() ?? throw new Exception("User does not exist or is not an admin!");

            if(admin.Clinic is null)
            {
                throw new Exception("Clinic for this admin could not be found!");
            }

            var invitaions = await _db.Invitations
                .Where(x => x.AdminId == adminId)
                .Select(x => new Models.Invitation.InvitationDto
                {
                    Id = x.Id,
                    AdminId = x.AdminId,
                    Status = x.Status,
                    DentistEmail = x.DentistEmail,
                    ClinicName = admin.Clinic.Name,
                }).ToListAsync();

            return invitaions;
        }

        public async Task<Models.Invitation.InvitationDto> SendInvitationToDentist(string email, int adminId)
        {
            var admin = await _db.Users
                .Where(x => x.Id == adminId && x.Role == Dental_Clinic.Enums.UserRole.Admin)
                .Include(x => x.Clinic)
                .FirstOrDefaultAsync() ?? throw new Exception("User does not exist or is not an admin!");

            if (admin.Clinic is null)
            {
                throw new Exception("Clinic for this admin could not be found!");
            }

            var existingInvitation = await _db.Invitations
                .Where(x => x.AdminId == adminId && x.DentistEmail == email)
                .FirstOrDefaultAsync();

            if (existingInvitation is not null)
            {
                if (existingInvitation.Status == Dental_Clinic.Enums.InvitationStatus.Accepted)
                {
                    throw new Exception("Invitation is already accepted!");
                }
                if (existingInvitation.Status == Dental_Clinic.Enums.InvitationStatus.Pending)
                {
                    throw new Exception("Invitation is already sent!");
                }
            }

            var invitationInDb = await _db.Invitations.AddAsync(new Infrastructure.Dtos.InvitationDto
            {
                DentistEmail = email,
                AdminId = adminId,
                Status = Dental_Clinic.Enums.InvitationStatus.Pending
            });

            await _db.SaveChangesAsync();

            return new Models.Invitation.InvitationDto
            {
                DentistEmail = email,
                AdminId = adminId,
                Id = invitationInDb.Entity.Id,
                ClinicName = admin.Clinic.Name,
                Status = Dental_Clinic.Enums.InvitationStatus.Pending
            }; ;
        }
    }
}
