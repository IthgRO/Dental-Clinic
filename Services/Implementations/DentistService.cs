using Dental_Clinic.Enums;
using Infrastructure;

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
}
