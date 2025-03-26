using Dental_Clinic.Enums;
using Infrastructure;
using Microsoft.EntityFrameworkCore;
using Services.Models.Dentist;

public class DentistService : IDentistService
{
    private readonly ApplicationContext _context;

    public DentistService(ApplicationContext context)
    {
        _context = context;
    }

    public string GetDentistNameById(int userId)
    {
        var user = _context.Users.FirstOrDefault(u => u.Id == userId && u.Role == UserRole.Dentist);

        // Return the name if the user is a dentist; otherwise, return "Unknown Dentist"
        return user != null ? $"{user.FirstName} {user.LastName}" : "Unknown Dentist";
    }

    public async Task RegisterDentist(DentistRegisterDto registerDto)
    {
        if (await _context.Users.AnyAsync(u => u.Email == registerDto.Email))
        {
            throw new Exception("Email already exists");
        }

        var invitation = await _context.Invitations
            .Where(x => x.DentistEmail == registerDto.Email)
            .FirstOrDefaultAsync() ?? throw new Exception("Contact your clinic admin to send you an invite on this email address!");

        if (invitation.Status == InvitationStatus.Accepted)
        {
            throw new Exception("The invitation has already been accepted!");
        }

        var admin = await _context.Users
            .Where(x => x.Id == invitation.AdminId)
            .Include(x => x.ClinicId)
            .FirstOrDefaultAsync() ?? throw new Exception("Clinic could not be found!");

        invitation.Status = InvitationStatus.Accepted;

        string hashedPassword = BCrypt.Net.BCrypt.HashPassword(registerDto.Password);

        _context.Users.Add(new Dental_Clinic.Dtos.UserDto
        { 
            Email = registerDto.Email,
            FirstName = registerDto.FirstName,
            LastName = registerDto.LastName,
            Password = hashedPassword,
            Phone = registerDto.Phone,
            Timezone = registerDto.Timezone,
            ClinicId = admin.ClinicId,
            Role=UserRole.Dentist,
            CreatedAt = DateTime.UtcNow,
        });

        await _context.SaveChangesAsync();
    }
}
