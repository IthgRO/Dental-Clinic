using Infrastructure;
using Services.Interfaces;

public class ServiceService : IServiceService
{
    private readonly ApplicationContext _context;

    public ServiceService(ApplicationContext context)
    {
        _context = context;
    }

    public string GetServiceNameById(int serviceId)
    {
        return _context.Services.FirstOrDefault(s => s.Id == serviceId)?.Name ?? "Unknown Service";
    }
}
