using Dental_Clinic.Enums;
using Infrastructure;
using Microsoft.EntityFrameworkCore;
using Services.Interfaces;
using Services.Models.Clinic;
using Services.Models.Reservation;
using Dental_Clinic.Dtos;

namespace Services.Implementations
{
    public class AppointmentService : IAppointmentService
    {
        private ApplicationContext _db;
        private readonly IClinicService _clinicService;
        public AppointmentService(ApplicationContext db, IClinicService clinicService)
        {
            _db = db;
            _clinicService = clinicService;
        }
        public async Task<List<FreeSlot>> GetAvailableTimeSlots(int dentistId, DateTime startDate, DateTime endDate)
        {
            var dentistFromDb = await _db.Users
                .Where(x => x.Id == dentistId && x.Role == Dental_Clinic.Enums.UserRole.Dentist)
                .Include(x => x.Clinic)
                .FirstOrDefaultAsync() ?? throw new Exception("Dentist cannot be identified!");

            if (dentistFromDb.Clinic == null)
                throw new Exception("Clinic not found for the dentist");

            var freeSlots = new List<FreeSlot>();

            var utcStartDate = ConvertToUtcFast(dentistFromDb.Clinic.Timezone, startDate);
            var utcEndDate = ConvertToUtcFast(dentistFromDb.Clinic.Timezone, endDate);

            var appointments = await _db.Appointments
                .Where(x => x.DentistId == dentistId &&
                            x.StartTime >= utcStartDate &&
                            x.EndTime <= utcEndDate &&
                            x.Status != Dental_Clinic.Enums.AppointmentStatus.Cancelled)
                .ToListAsync();

            var schedules = await _db.DentistSchedules
                .Where(ds => ds.DentistId == dentistId)
                .ToListAsync();

            for (var date = startDate.Date; date <= endDate.Date; date = date.AddDays(1))
            {
                var daySchedule = schedules
                    .FirstOrDefault(s => s.DayOfWeek == date.DayOfWeek);

                if (daySchedule == null)
                    continue;

                var currentStartTime = date.Date.Add(daySchedule.StartTime);
                var currentEndTime = date.Date.Add(daySchedule.EndTime);

                if (date.Date == startDate.Date && startDate > currentStartTime)
                    currentStartTime = startDate;

                if (date.Date == endDate.Date && endDate < currentEndTime)
                    currentEndTime = endDate;

                for (var candidateTime = currentStartTime; candidateTime < currentEndTime; candidateTime = candidateTime.AddMinutes(30))
                {
                    var candidateEndTime = candidateTime.AddMinutes(30);

                    if (candidateEndTime > currentEndTime)
                        break;

                    var utcCandidateStart = ConvertToUtcFast(dentistFromDb.Clinic.Timezone, candidateTime);
                    var utcCandidateEnd = ConvertToUtcFast(dentistFromDb.Clinic.Timezone, candidateEndTime);

                    var isTaken = appointments.Any(apt =>
                        utcCandidateStart < apt.EndTime && utcCandidateEnd > apt.StartTime);

                    if (!isTaken)
                    {
                        freeSlots.Add(new FreeSlot
                        {
                            StartTime = candidateTime,
                            EndTime = candidateEndTime
                        });
                    }
                }
            }

            return freeSlots;
        }


        public async Task<Dental_Clinic.Dtos.AppointmentDto> BookAppointment(int userId, int dentistId, int serviceId, int clinicId, DateTime startDate)
        {
            await ValidateIfUserIsDentist(dentistId);


            if (startDate.Hour < 9 || startDate.Hour > 17)
            {
                throw new Exception("Dentist is not working during this time interval!");
            }
            else if (startDate.DayOfWeek == DayOfWeek.Saturday || startDate.DayOfWeek == DayOfWeek.Sunday)
            {
                throw new Exception("The dentist is not working on weekends!");
            }

            startDate = SetToFixHour(startDate);

            var clinic = await _db.Clinics.FirstOrDefaultAsync(c => c.Id == clinicId);
            if (clinic == null)
            {
                throw new Exception("Clinic not found.");
            }
            // Convert StartTime to UTC
            var clinicTimeZoneId = _clinicService.ConvertClinicTimezoneEnumToWindows(clinic.Timezone);
            var utcStart = _clinicService.ConvertToUtc(clinicId, startDate);

            await ValidateIfDentistIsFree(dentistId, utcStart);

            var newAppointment = new Dental_Clinic.Dtos.AppointmentDto
            {
                ClinicId = clinicId,
                DentistId = dentistId,
                PatientId = userId,
                ServiceId = serviceId,
                StartTime = utcStart, // Store the time in UTC
                EndTime = utcStart.AddHours(1),
                Timezone = clinicTimeZoneId,
                Status = AppointmentStatus.Pending,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            _db.Appointments.Add(newAppointment);
            await _db.SaveChangesAsync();
            return newAppointment;
        }

        private async Task ValidateIfUserIsDentist(int dentistId)
        {
            var isDentist = await _db.Users.Where(x => x.Id == dentistId && x.Role == Dental_Clinic.Enums.UserRole.Dentist).AnyAsync();

            if (!isDentist)
            {
                throw new Exception("Dentist cannot be identified!");
            }
        }

        private async Task ValidateIfDentistIsFree(int dentistId, DateTime startDate)
        {
            var existingReservation = await _db.Appointments.Where(x => x.DentistId == dentistId && x.StartTime == startDate && x.Status != AppointmentStatus.Cancelled).AnyAsync();

            if (existingReservation)
            {
                throw new Exception("The slot is not available for this dentist!");
            }
        }

        public async Task<List<Models.Reservation.AppointmentDto>> GetMyAppointments(int userId)
        {
            var appointments = await _db.Appointments
                .Where(x => x.PatientId == userId && x.Status != Dental_Clinic.Enums.AppointmentStatus.Cancelled)
                .Include(x => x.Dentist)
                .Include(x => x.Clinic)
                .Include(x => x.Service)
                .ToListAsync();


            return appointments.Select(x => new Models.Reservation.AppointmentDto
            {
                Id = x.Id,
                DentistId = x.DentistId,
                ClinicName = x.Clinic.Name,
                City = x.Clinic.City,
                DentistFirstName = x.Dentist.FirstName,
                DentistLastName = x.Dentist.LastName,
                ServiceName = x.Service.Name,
                Currency = x.Service.Currency,
                ServicePrice = x.Service.Price,
                StartTime = ConvertToLocalFast(x.Clinic.Timezone, x.StartTime),
                EndTime = ConvertToLocalFast(x.Clinic.Timezone, x.EndTime),
                Status = x.Status,
            }).ToList();
        }

        public async Task CancelAppointment(int userId, int appointmentId)
        {
            var appointmentFromDb = await _db.Appointments
                .Where(x => x.Id == appointmentId)
                .FirstOrDefaultAsync();

            if (appointmentFromDb == null)
            {
                throw new Exception("Appointment could not be found! ");
            }

            else if (appointmentFromDb.PatientId != userId && appointmentFromDb.DentistId != userId)
            {
                throw new Exception("User not allowed to cancel this appointment!");
            }

            appointmentFromDb.Status = Dental_Clinic.Enums.AppointmentStatus.Cancelled;
            await _db.SaveChangesAsync();
        }

        public async Task ConfirmAppointment(int userId, int appointmentId)
        {
            var appointmentFromDb = await _db.Appointments
                .Where(x => x.Id == appointmentId)
                .FirstOrDefaultAsync();

            if (appointmentFromDb == null)
            {
                throw new Exception("Appointment could not be found! ");
            }

            else if (appointmentFromDb.DentistId != userId)
            {
                throw new Exception("User not allowed to confirm this appointment!");
            }

            appointmentFromDb.Status = Dental_Clinic.Enums.AppointmentStatus.Confirmed;
            await _db.SaveChangesAsync();
        }

        public async Task UpdateAppointment(int userId, int appointmentId, DateTime newDate)
        {
            newDate = SetToFixHour(newDate);

            if (newDate.Hour < 9 || newDate.Hour > 17)
            {
                throw new Exception("Dentist is not working during this time interval!");
            }
            else if (newDate.DayOfWeek == DayOfWeek.Saturday || newDate.DayOfWeek == DayOfWeek.Sunday)
            {
                throw new Exception("The dentist is not working on weekends!");
            }

            var appointmentFromDb = await _db.Appointments
                .Where(x => x.Id == appointmentId)
                .FirstOrDefaultAsync();

            if (appointmentFromDb == null)
            {
                throw new Exception("Appointment could not be found! ");
            }

            else if (appointmentFromDb.PatientId != userId)
            {
                throw new Exception("Appointment does not belong to this user! ");
            }

            var dentistFromDb = await _db.Users
                .Where(x => x.Id == appointmentFromDb.DentistId && x.Role == Dental_Clinic.Enums.UserRole.Dentist)
                .Include(x => x.Clinic)
                .FirstOrDefaultAsync();

            if (dentistFromDb == null)
            {
                throw new Exception("This appointment's dentist cannot be found!");
            }

            else if (dentistFromDb.Clinic == null)
            {
                throw new Exception("This appointment's clinic cannot be found!");
            }

            var clinicTimeZoneId = _clinicService.ConvertClinicTimezoneEnumToWindows(dentistFromDb.Clinic.Timezone);
            var utcStart = ConvertToUtcFast(dentistFromDb.Clinic.Timezone, newDate);

            await ValidateIfDentistIsFree(appointmentFromDb.DentistId, utcStart);

            var reminderFromDb = await _db.Reminders
                .Where(x => x.AppointmentId == appointmentId)
                .FirstOrDefaultAsync();

            if (reminderFromDb is not null && reminderFromDb.Status != ReminderStatus.Sent)
            {
                reminderFromDb.SendAt = utcStart.AddHours(-1);
                reminderFromDb.UpdatedAt = DateTime.UtcNow;
            }

            else if (reminderFromDb is not null && reminderFromDb.Status == ReminderStatus.Sent)
            {
                throw new Exception("It is too late for you to change the appointment!");
            }

            appointmentFromDb.StartTime = utcStart;
            appointmentFromDb.EndTime = utcStart.AddHours(1);

            await _db.SaveChangesAsync();
        }

        private DateTime SetToFixHour(DateTime date)
        {
            DateTime updatedDateTime = new DateTime(
            date.Year,
            date.Month,
            date.Day,
            date.Hour,
            0,
            0
            );

            return updatedDateTime;
        }

        public async Task<List<DentistAppointmentDto>> GetDentistAppointments(int dentistId)
        {
            await ValidateIfUserIsDentist(dentistId);

            var appointmentsFromDb = await (from appt in _db.Appointments
                                            join user in _db.Users
                                            on appt.PatientId equals user.Id
                                            join service in _db.Services
                                            on appt.ServiceId equals service.Id
                                            join clinic in _db.Clinics
                                            on appt.ClinicId equals clinic.Id
                                            where appt.DentistId == dentistId
                                            && appt.Status != Dental_Clinic.Enums.AppointmentStatus.Cancelled
                                            select new DentistAppointmentDto
                                            {
                                                Id = appt.Id,
                                                FirstName = user.FirstName,
                                                LastName = user.LastName,
                                                ServiceName = service.Name,
                                                StartTime = appt.StartTime,
                                                EndTime =  appt.EndTime,
                                                Status = appt.Status,
                                                TimeZone = appt.Clinic.Timezone
                                                
                                            }).ToListAsync();

            foreach(var appointment in appointmentsFromDb)
            {
                appointment.StartTime = ConvertToLocalFast(appointment.TimeZone, appointment.StartTime);
                appointment.EndTime = ConvertToLocalFast(appointment.TimeZone, appointment.EndTime);
            }
            return appointmentsFromDb;
        }

        private DateTime ConvertToUtcFast(ClinicTimezone timezone, DateTime localTime)
        {


            // If the DateTime is already in UTC, return it directly
            if (localTime.Kind == DateTimeKind.Utc)
            {
                return localTime;
            }

            var clinicTimeZoneId = _clinicService.ConvertClinicTimezoneEnumToWindows(timezone);
            var clinicTimeZone = TimeZoneInfo.FindSystemTimeZoneById(clinicTimeZoneId);

            // Ensure the input DateTime is treated as Unspecified before conversion
            var clinicLocalTime = DateTime.SpecifyKind(localTime, DateTimeKind.Unspecified);
            var utcTime = TimeZoneInfo.ConvertTimeToUtc(clinicLocalTime, clinicTimeZone);

            return utcTime;
        }

        private DateTime ConvertToLocalFast(ClinicTimezone timezone, DateTime utcTime)
        {

            var clinicTimeZoneId = _clinicService.ConvertClinicTimezoneEnumToWindows(timezone);
            var clinicTimeZone = TimeZoneInfo.FindSystemTimeZoneById(clinicTimeZoneId);

            var utcDateTime = DateTime.SpecifyKind(utcTime, DateTimeKind.Utc);
            var localTime = TimeZoneInfo.ConvertTimeFromUtc(utcDateTime, clinicTimeZone);

            return localTime;
        }

        public async Task<Dental_Clinic.Dtos.AppointmentDto> GetAppointmentByIdAsync(int appointmentId)
        {
            var appointment = await _db.Appointments
                .Include(a => a.Clinic)
                .Include(a => a.Dentist)
                .Include(a => a.Service)
                .FirstOrDefaultAsync(a => a.Id == appointmentId);

            if (appointment == null)
            {
                throw new Exception("Appointment not found.");
            }

            return appointment;
        }
    }
}