using Services.Models.Service;

namespace Services.Interfaces;

public interface IServiceService
{
    string GetServiceNameById(int ServiceId);

    public Task AddServiceToDentist(ServiceDto serviceDto);

    public Task<IEnumerable<ServiceDto>> GetServicesForDentist(int dentistId);

    public Task UpdateServicesForDentist(List<ServiceDto> services, int dentistId);
}
