using Infrastructure;
using Microsoft.EntityFrameworkCore;
using Dental_Clinic.Dtos;
using System;
using Dental_Clinic.Enums;
using Microsoft.Extensions.Logging;
using Services.Models.Clinic;
using Microsoft.IdentityModel.Tokens;


public class ClinicService : IClinicService
{
    private readonly ApplicationContext _context;
    private readonly ILogger<ClinicService> _logger;

    public ClinicService(ApplicationContext context, ILogger<ClinicService> logger)
    {
        _context = context;
        _logger = logger;
    }

    public string GetClinicNameById(int clinicId)
    {
        return _context.Clinics.FirstOrDefault(c => c.Id == clinicId)?.Name ?? "Unknown Clinic";
    }

    public async Task<ClinicDto?> GetClinicByIdAsync(int clinicId)
    {
        return await _context.Clinics
            .Where(c => c.Id == clinicId)
            .Select(c => new ClinicDto
            {
                Id = c.Id,
                Name = c.Name,
                Address = c.Address,
                City = c.City,
                Country = c.Country,
                Timezone = c.Timezone, // Fetching Timezone from DB
                Phone = c.Phone,
                Email = c.Email,
                IsActive = c.IsActive,
                CreatedAt = c.CreatedAt,
                UpdatedAt = c.UpdatedAt
            })
            .FirstOrDefaultAsync();
    }

    public DateTime ConvertToUtc(int clinicId, DateTime localTime)
    {
        var clinic = _context.Clinics.FirstOrDefault(c => c.Id == clinicId);
        if (clinic == null)
        {
            throw new Exception("Clinic not found.");
        }

        // If the DateTime is already in UTC, return it directly
        if (localTime.Kind == DateTimeKind.Utc)
        {
            _logger.LogInformation($"Skipping conversion: Input time is already UTC ({localTime}).");
            return localTime;
        }

        var clinicTimeZoneId = ConvertClinicTimezoneEnumToWindows(clinic.Timezone);
        var clinicTimeZone = TimeZoneInfo.FindSystemTimeZoneById(clinicTimeZoneId);

        _logger.LogInformation($"Converting time for Clinic ID {clinicId}: LocalTime: {localTime} (Kind: {localTime.Kind}), Timezone: {clinicTimeZoneId}");

        // Ensure the input DateTime is treated as Unspecified before conversion
        var clinicLocalTime = DateTime.SpecifyKind(localTime, DateTimeKind.Unspecified);
        var utcTime = TimeZoneInfo.ConvertTimeToUtc(clinicLocalTime, clinicTimeZone);

        _logger.LogInformation($"Converted {clinicLocalTime} to UTC: {utcTime}");
        return utcTime;
    }



    public string ConvertClinicTimezoneEnumToWindows(ClinicTimezone clinicTimezone)
    {
        return clinicTimezone switch
        {
            ClinicTimezone.EasternStandardTime => "Eastern Standard Time",
            ClinicTimezone.CentralStandardTime => "Central Standard Time",
            ClinicTimezone.MountainStandardTime => "Mountain Standard Time",
            ClinicTimezone.PacificStandardTime => "Pacific Standard Time",
            ClinicTimezone.AlaskaStandardTime => "Alaskan Standard Time",
            ClinicTimezone.HawaiiAleutianStandardTime => "Hawaiian Standard Time",
            ClinicTimezone.ArgentinaStandardTime => "Argentina Standard Time",
            ClinicTimezone.BrasiliaStandardTime => "E. South America Standard Time",
            ClinicTimezone.ChileStandardTime => "Pacific SA Standard Time",
            ClinicTimezone.GreenwichMeanTime => "GMT Standard Time",
            ClinicTimezone.CentralEuropeanTime => "Central Europe Standard Time",
            ClinicTimezone.EasternEuropeanTime => "E. Europe Standard Time",
            ClinicTimezone.SouthAfricaStandardTime => "South Africa Standard Time",
            ClinicTimezone.EgyptStandardTime => "Egypt Standard Time",
            ClinicTimezone.WestAfricaStandardTime => "W. Central Africa Standard Time",
            ClinicTimezone.IndiaStandardTime => "India Standard Time",
            ClinicTimezone.ChinaStandardTime => "China Standard Time",
            ClinicTimezone.JapanStandardTime => "Tokyo Standard Time",
            ClinicTimezone.KoreaStandardTime => "Korea Standard Time",
            ClinicTimezone.IndochinaTime => "SE Asia Standard Time",
            ClinicTimezone.ArabianStandardTime => "Arab Standard Time",
            ClinicTimezone.AustralianEasternStandardTime => "AUS Eastern Standard Time",
            ClinicTimezone.AustralianCentralStandardTime => "Cen. Australia Standard Time",
            ClinicTimezone.AustralianWesternStandardTime => "W. Australia Standard Time",
            ClinicTimezone.CoordinatedUniversalTime => "UTC",
            _ => "UTC"
        };
    }

    public async Task<Clinic> GetDentistClinic(int dentistId)
    {
        var dentistFromDb = await _context.Users
            .Include(x => x.Clinic)
            .Where(x => x.Id == dentistId)
            .FirstOrDefaultAsync();

        ValidateDentistClinic(dentistFromDb);

        return new Clinic
        {
            Address = dentistFromDb.Clinic.Address,
            Name = dentistFromDb.Clinic.Name,
            Timezone = dentistFromDb.Clinic.Timezone,
            City = dentistFromDb.Clinic.City,
            Country = dentistFromDb.Clinic.Country,
            Phone = dentistFromDb.Clinic.Phone,
            Email = dentistFromDb.Clinic.Email
        };

    }

    private void ValidateDentistClinic(UserDto dentist)
    {


        if (dentist == null || dentist.Role == UserRole.Patient)
        {
            throw new Exception("Only doctors can manage their clinic!");
        }

        else if (dentist.Clinic == null)
        {
            throw new Exception("This user is not associated with a clinic!");
        }
    }

    public async Task UpdateClinicAddress(int dentistId, string address)
    {
        var dentistFromDb = await _context.Users
            .Include(x => x.Clinic)
            .Where(x => x.Id == dentistId)
            .FirstOrDefaultAsync();

        ValidateDentistClinic(dentistFromDb);

        if (address == string.Empty)
        {
            throw new Exception("New Address cannot be empty!");
        }

        dentistFromDb.Clinic.Address = address;
        await _context.SaveChangesAsync();
    }

    public async Task UploadClinicPicture(int dentistId, byte[] picture, string pictureFormat)
    {
        var dentistFromDb = await _context.Users
            .Include(x => x.Clinic)
            .Where(x => x.Id == dentistId)
            .FirstOrDefaultAsync();

        ValidateDentistClinic(dentistFromDb);

        if (dentistFromDb.Clinic == null)
        {
            throw new Exception("No clinic found for this dentist!");
        }

        dentistFromDb.Clinic.Picture = picture;
        dentistFromDb.Clinic.PictureFormat = pictureFormat;

        await _context.SaveChangesAsync();
    }

    public async Task<ClinicPicture> GetClinicPicture(int dentistId)
    {
        var dentistFromDb = await _context.Users
            .Include(x => x.Clinic)
            .Where(x => x.Id == dentistId)
            .FirstOrDefaultAsync();

        ValidateDentistClinic(dentistFromDb);

        if (dentistFromDb.Clinic == null)
        {
            throw new Exception("No clinic found for this dentist!");
        }

        if(dentistFromDb.Clinic.Picture == null || dentistFromDb.Clinic.PictureFormat == null)
        {
            throw new Exception("Picture of clinic not found!");
        }

        return new ClinicPicture
        {
            Format = dentistFromDb.Clinic.PictureFormat,
            Picture = dentistFromDb.Clinic.Picture
        };

    }
    // Generates a unique 6-digit clinic code
    public string GenerateUniqueClinicCode()
    {
        string code;
        Random rnd = new Random();
        do
        {
            // Generate a 6-digit code (leading zeros included)
            code = rnd.Next(0, 1000000).ToString("D6");
        }
        while (_context.Clinics.Any(c => c.ClinicCode == code));

        return code;
    }
    public async Task<ClinicDto?> GetClinicByCodeAsync(string clinicCode)
    {
        return await _context.Clinics
            .Where(c => c.ClinicCode == clinicCode)
            .Select(c => new ClinicDto
            {
                Id = c.Id,
                Name = c.Name,
                Address = c.Address,
                City = c.City,
                Country = c.Country,
                Timezone = c.Timezone,
                Phone = c.Phone,
                Email = c.Email,
                IsActive = c.IsActive,
                CreatedAt = c.CreatedAt,
                UpdatedAt = c.UpdatedAt,
                Picture = c.Picture,
                PictureFormat = c.PictureFormat,
                ClinicCode = c.ClinicCode,
                Users = c.Users,
                Appointments = c.Appointments
            })
            .FirstOrDefaultAsync();
    }
}
