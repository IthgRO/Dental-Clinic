using Infrastructure;
public class ClinicService : IClinicService
{
    private readonly ApplicationContext _context;

    public ClinicService(ApplicationContext context)
    {
        _context = context;
    }

    public string GetClinicNameById(int clinicId)
    {
        return _context.Clinics.FirstOrDefault(c => c.Id == clinicId)?.Name ?? "Unknown Clinic";
    }
}
