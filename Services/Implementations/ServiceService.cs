using Infrastructure;
using Microsoft.EntityFrameworkCore;
using Services.Interfaces;
using Services.Models.Service;

public class ServiceService : IServiceService
{
    private readonly ApplicationContext _context;

    public ServiceService(ApplicationContext context)
    {
        _context = context;
    }

    public async Task AddServiceToDentist(Services.Models.Service.ServiceDto serviceDto)
    {
        await ValidateDentist(serviceDto.DentistId);
        await ValidateService(serviceDto.Name, serviceDto.DentistId);

        var newService = new Dental_Clinic.Dtos.ServiceDto
        {
            UserId = serviceDto.DentistId,
            Name = serviceDto.Name,
            Currency = "",
            Category = serviceDto.Category,
            Description = "",
            Duration = serviceDto.Duration,
        };

        _context.Services.Add(newService);
        await _context.SaveChangesAsync();
    }

    public string GetServiceNameById(int serviceId)
    {
        return _context.Services.FirstOrDefault(s => s.Id == serviceId)?.Name ?? "Unknown Service";
    }

    public async Task<IEnumerable<ServiceDto>> GetServicesForDentist(int dentistId)
    {
        await ValidateDentist(dentistId);

        var servicesFromDb = await _context.Services
            .Where(x => x.UserId == dentistId)
            .ToListAsync();

        return servicesFromDb.Select(x => new ServiceDto
        {
            Category = x.Category,
            Name = x.Name,
            Duration = x.Duration,
            DentistId = dentistId
        });
    }

    public async Task UpdateServicesForDentist(List<ServiceDto> services, int dentistId)
    {
        await ValidateDentist(dentistId);

        var servicesFromDb = await _context.Services
            .Where(x => x.UserId == dentistId)
            .ToListAsync();

        _context.Services.RemoveRange(servicesFromDb);

        foreach (var service in services)
        {
            _context.Services.Add(new Dental_Clinic.Dtos.ServiceDto
            {
                Category = service.Category,
                Name = service.Name,
                Duration = service.Duration,
                UserId = dentistId,
                Currency = "",
                Description = "",
            });
        }
        await _context.SaveChangesAsync();
    }

    private async Task ValidateDentist(int dentistId)
    {
        var userFromDb = await _context.Users
            .Where(x => x.Id == dentistId)
            .FirstOrDefaultAsync();

        if (userFromDb == null)
        {
            throw new Exception("User could not be found!");
        }

        else if (userFromDb.Role != Dental_Clinic.Enums.UserRole.Dentist && userFromDb.Role != Dental_Clinic.Enums.UserRole.Admin)
        {
            throw new Exception("Only dentists are allowed access to their services!");
        }
    }

    private async Task ValidateService(string serviceName, int dentistId)
    {
        var serviceFromDb = await _context.Services
            .Where(x => x.Name == serviceName && x.UserId == dentistId)
            .FirstOrDefaultAsync();

        if (serviceFromDb is not null)
        {
            throw new Exception("This dentist already has this service associated!");
        }
    }
}
