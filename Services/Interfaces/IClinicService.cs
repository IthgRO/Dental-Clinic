using Dental_Clinic.Dtos;
using Dental_Clinic.Enums;
using Services.Models.Clinic;

public interface IClinicService
{
    string GetClinicNameById(int clinicId);
    public Task<Clinic> GetDentistClinic(int dentistId);
    Task<ClinicDto?> GetClinicByIdAsync(int clinicId);
    DateTime ConvertToUtc(int clinicId, DateTime localTime);
    string ConvertClinicTimezoneEnumToWindows(ClinicTimezone clinicTimezone);
}

