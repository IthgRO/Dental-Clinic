using Dental_Clinic.Dtos;
using Dental_Clinic.Enums;

public interface IClinicService
{
    string GetClinicNameById(int clinicId);
    Task<ClinicDto?> GetClinicByIdAsync(int clinicId);
    DateTime ConvertToUtc(int clinicId, DateTime localTime);
    string ConvertClinicTimezoneEnumToWindows(ClinicTimezone clinicTimezone);
}

