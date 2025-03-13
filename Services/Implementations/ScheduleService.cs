using Dental_Clinic.Dtos;
using Dental_Clinic.Enums;
using Infrastructure;
using Microsoft.EntityFrameworkCore;
using Services.Interfaces;
using Services.Models.Schedule;

namespace Services.Implementations
{
    public class ScheduleService : IScheduleService
    {
        private readonly ApplicationContext _db;
        public ScheduleService(ApplicationContext db)
        {
            _db = db;
        }

        public async Task AddSchedule(int dentistId, TimeSpan startTime, TimeSpan endTime, DayOfWeek dayOfWeek)
        {
            var dentistFromDb = await GetDentistFromDb(dentistId);

            startTime = ConvertToUtcFast(dentistFromDb.Clinic.Timezone, startTime);
            endTime = ConvertToUtcFast(dentistFromDb.Clinic.Timezone, endTime);

            var existingSchedules = await _db.DentistSchedules
                .Where(x => x.DentistId == dentistId)
                .ToListAsync();

            foreach (var existingSchedule in existingSchedules)
            {
                if (existingSchedule.StartTime > startTime && existingSchedule.StartTime < endTime)
                {
                    throw new Exception("This slot already belongs to a schedule!");
                }
                else if (existingSchedule.EndTime > startTime && existingSchedule.EndTime < endTime)
                {
                    throw new Exception("This slot already belongs to a schedule!");
                }
                else if (existingSchedule.StartTime == startTime && existingSchedule.EndTime == endTime)
                {
                    throw new Exception("This slot already belongs to a schedule!");
                }
            }

            _db.DentistSchedules.Add(new DentistScheduleDto
            {
                DentistId = dentistId,
                DayOfWeek = dayOfWeek,
                StartTime = startTime,
                EndTime = endTime,
                ClinicId = dentistFromDb.Clinic.Id
            });

            await _db.SaveChangesAsync();
        }

        private TimeSpan ConvertToUtcFast(ClinicTimezone timezone, TimeSpan localTime)
        {
            var clinicTimeZoneId = ConvertClinicTimezoneEnumToWindows(timezone);
            var clinicTimeZone = TimeZoneInfo.FindSystemTimeZoneById(clinicTimeZoneId);

            TimeSpan currentOffset = clinicTimeZone.GetUtcOffset(DateTime.UtcNow);

            TimeSpan utcTime = localTime - currentOffset;
            return utcTime;
        }

        private TimeSpan ConvertToLocalFast(ClinicTimezone timezone, TimeSpan localTime)
        {
            var clinicTimeZoneId = ConvertClinicTimezoneEnumToWindows(timezone);
            var clinicTimeZone = TimeZoneInfo.FindSystemTimeZoneById(clinicTimeZoneId);

            TimeSpan currentOffset = clinicTimeZone.GetUtcOffset(DateTime.UtcNow);

            TimeSpan utcTime = localTime + currentOffset;
            return utcTime;
        }


        public static string ConvertClinicTimezoneEnumToWindows(ClinicTimezone clinicTimezone)
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

        private async Task<UserDto> GetDentistFromDb(int dentistId)
        {
            var dentistFromDb = await _db.Users
                .Where(x => x.Id == dentistId)
                .Include(x => x.Clinic)
                .FirstOrDefaultAsync() ?? throw new Exception("User could not be found!");

            if (dentistFromDb.Role == Dental_Clinic.Enums.UserRole.Patient)
            {
                throw new Exception("Patients cannot update their schedule!");
            }

            return dentistFromDb;
        }

        public async Task DeleteSchedule(int dentistId, int scheduleId)
        {
            var dentistFromDb = await GetDentistFromDb(dentistId);

            var schedule = await _db.DentistSchedules
                .Where(x => x.Id == scheduleId)
                .FirstOrDefaultAsync();

            if (schedule is null || schedule.DentistId != dentistId)
            {
                throw new Exception("Schedule does not exist or does not belong to this dentist!");
            }

            _db.DentistSchedules.Remove(schedule);
            await _db.SaveChangesAsync();
        }

        public async Task<List<ScheduleDto>> GetDentistSchedules(int dentistId)
        {
            var dentistFromDb = await _db.Users
                .Where(x => x.Id == dentistId)
                .Include(x => x.Clinic)
                .FirstOrDefaultAsync() ?? throw new Exception("User could not be found!");

            if (dentistFromDb.Role == Dental_Clinic.Enums.UserRole.Patient)
            {
                throw new Exception("Patients cannot see their schedule!");
            }

            if (dentistFromDb.Clinic is null)
            {
                throw new Exception("Clinic for this dentist cannot be found!");
            }

            var schedules = await _db.DentistSchedules
                .Where(x => x.DentistId == dentistId)
                .OrderBy(x => x.DayOfWeek)
                .ThenBy(x => x.StartTime)
                .Select(x => new ScheduleDto
                {
                    StartTime = x.StartTime,
                    EndTime = x.EndTime,
                    DayOfWeek = x.DayOfWeek,
                }).ToListAsync();

            foreach (var schedule in schedules)
            {
                schedule.StartTime = ConvertToLocalFast(dentistFromDb.Clinic.Timezone, schedule.StartTime);
                schedule.EndTime = ConvertToLocalFast(dentistFromDb.Clinic.Timezone, schedule.EndTime);
            }

            return schedules;
        }
    }
}
