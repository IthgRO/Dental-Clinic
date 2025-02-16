using Dental_Clinic.Dtos;
using Dental_Clinic.Enums;
using Org.BouncyCastle.Crypto.Paddings;
using Services.Models.Clinic;

public interface IClinicService
{
    string GetClinicNameById(int clinicId);
    public Task<Clinic> GetDentistClinic(int dentistId);
    Task<ClinicDto?> GetClinicByIdAsync(int clinicId);
    Task UpdateClinicAddress(int dentistId, string address);
    DateTime ConvertToUtc(int clinicId, DateTime localTime);
    string ConvertClinicTimezoneEnumToWindows(ClinicTimezone clinicTimezone);
    Task UploadClinicPicture(int dentistId, byte[] picture, string pictureType);
    Task<ClinicPicture> GetClinicPicture(int dentistId);
}

