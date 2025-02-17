﻿using Dental_Clinic.Enums;
using Infrastructure;
using Microsoft.EntityFrameworkCore;
using Services.Interfaces;
using Services.Models.Reservation;
using System.ComponentModel.DataAnnotations;
using static System.Runtime.InteropServices.JavaScript.JSType;

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
            await ValidateIfUserIsDentist(dentistId);

            var freeSlots = new List<FreeSlot>();

            startDate = SetToFixHour(startDate);
            endDate = SetToFixHour(endDate);

            var appointments = await _db.Appointments
                .Where(x => x.DentistId == dentistId && x.StartTime >= startDate && x.EndTime <= endDate && x.Status != Dental_Clinic.Enums.AppointmentStatus.Cancelled)
                .ToListAsync();

            for (DateTime date = startDate.Date; date <= endDate.Date; date = date.AddDays(1))
            {
                if (date.DayOfWeek != DayOfWeek.Saturday && date.DayOfWeek != DayOfWeek.Sunday)
                {
                    var default_start_date = TimeSpan.FromHours(9);
                    var default_end_date = TimeSpan.FromHours(17);

                    if (date == startDate.Date)
                    {
                        default_start_date = TimeSpan.FromHours(Math.Max(startDate.Hour, 9));
                    }
                    if (date == endDate.Date)
                    {
                        default_end_date = TimeSpan.FromHours(Math.Min(endDate.Hour, 17));
                    }

                    for (var candidateHour = default_start_date; candidateHour < default_end_date; candidateHour += TimeSpan.FromHours(1))
                    {
                        var candidateDate = date + candidateHour;
                        var isTaken = appointments.Any(date => date.StartTime == candidateDate);

                        if (!isTaken)
                        {
                            freeSlots.Add(new FreeSlot { StartTime = candidateDate, EndTime = candidateDate.AddHours(1) });
                        }
                    }
                }
            }
            return freeSlots;
        }

        public async Task<Dental_Clinic.Dtos.AppointmentDto> BookAppointment(int userId, int dentistId, int serviceId, int clinicId, DateTime startDate)
        {
            await ValidateIfUserIsDentist(dentistId);

            await ValidateIfDentistIsFree(dentistId, startDate);

            var clinic = await _db.Clinics.FirstOrDefaultAsync(c => c.Id == clinicId);
            if (clinic == null)
            {
                throw new Exception("Clinic not found.");
            }
            // Convert StartTime to UTC
            var clinicTimeZoneId = _clinicService.ConvertClinicTimezoneEnumToWindows(clinic.Timezone);
            var utcStart = _clinicService.ConvertToUtc(clinicId, startDate);

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

            else if (startDate.Hour < 9 || startDate.Hour > 17)
            {
                throw new Exception("Dentist is not working during this time interval!");
            }
            else if (startDate.DayOfWeek == DayOfWeek.Saturday || startDate.DayOfWeek == DayOfWeek.Sunday)
            {
                throw new Exception("The dentist is not working on weekends!");
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
                StartTime = x.StartTime,
                EndTime = x.EndTime
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

            else if (appointmentFromDb.PatientId != userId)
            {
                throw new Exception("Appointment does not belong to this user! ");
            }

            appointmentFromDb.Status = Dental_Clinic.Enums.AppointmentStatus.Cancelled;
            await _db.SaveChangesAsync();
        }

        public async Task UpdateAppointment(int userId, int appointmentId, DateTime newDate)
        {
            newDate = SetToFixHour(newDate);

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

            await ValidateIfDentistIsFree(appointmentFromDb.DentistId, newDate);

            var reminderFromDb = await _db.Reminders
                .Where(x => x.AppointmentId == appointmentId)
                .FirstOrDefaultAsync();

            if (reminderFromDb == null)
            {
                throw new Exception("The appointment for the reminder could not be found!");
            }

            else if (reminderFromDb.Status == ReminderStatus.Sent)
            {
                throw new Exception("It is too late for you to change the appointment!");
            }

            appointmentFromDb.StartTime = newDate;
            appointmentFromDb.EndTime = newDate.AddHours(1);
            reminderFromDb.SendAt = newDate.AddHours(-1);
            reminderFromDb.UpdatedAt = DateTime.UtcNow;

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
                                            where appt.DentistId == dentistId
                                            && appt.Status != Dental_Clinic.Enums.AppointmentStatus.Cancelled
                                            select new DentistAppointmentDto
                                            {
                                                Id = appt.Id,
                                                FirstName = user.FirstName,
                                                LastName = user.LastName,
                                                ServiceName = service.Name, 
                                                StartTime = appt.StartTime,
                                                EndTime = appt.EndTime,
                                            }).ToListAsync();
            return appointmentsFromDb;
        }
    }
}