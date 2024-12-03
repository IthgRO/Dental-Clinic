using Dental_Clinic.Dtos;
using Infrastructure;
using Microsoft.EntityFrameworkCore;
using Services.Interfaces;
using Services.Models.Reservation;

namespace Services.Implementations
{
    public class AppointmentService : IAppointmentService
    {
        private ApplicationContext _db;
        public AppointmentService(ApplicationContext db)
        {
            _db = db;
        }
        public async Task<List<FreeSlot>> GetAvailableTimeSlots(int dentistId, DateTime startDate, DateTime endDate)
        {
            await ValidateIfUserIsDentist(dentistId);

            var freeSlots = new List<FreeSlot>();

            startDate = startDate.Date;
            endDate = endDate.Date;

            var appointments = await _db.Appointments
                .Where(x => x.DentistId == dentistId && x.StartTime >= startDate && x.EndTime <= endDate)
                .ToListAsync();

            for (DateTime date = startDate; date <= endDate; date = date.AddDays(1))
            {
                var default_start_time = TimeSpan.FromHours(9);
                var default_end_date = TimeSpan.FromHours(17);

                for (var candidateHour = default_start_time; candidateHour < default_end_date; candidateHour += TimeSpan.FromHours(1))
                {
                    var candidateDate = date + candidateHour;
                    var isTaken = appointments.Any(date => date.StartTime == candidateDate);

                    if (!isTaken)
                    {
                        freeSlots.Add(new FreeSlot { StartTime = candidateDate, EndTime = candidateDate.AddHours(1) });
                    }
                }
            }
            return freeSlots;
        }

        public async Task BookAppointment(int userId, int dentistId, int serviceId, int clinicId, DateTime startDate)
        {
            await ValidateIfUserIsDentist(dentistId);

            await ValidateIfDentistIsFree(dentistId, startDate);

            var newAppointment = new AppointmentDto
            {
                ClinicId = clinicId,
                DentistId = dentistId,
                PatientId = userId,
                StartTime = startDate,
                EndTime = startDate.AddHours(1),
                Timezone = "",
                CreatedAt = DateTime.UtcNow,

            };

            _db.Appointments.Add(newAppointment);
            await _db.SaveChangesAsync();
        }

        private async Task ValidateIfUserIsDentist(int dentistId)
        {
            var isDentist = await _db.Users.Where(x => x.Id == dentistId && x.Role ==Dental_Clinic.Enums.UserRole.Dentist).AnyAsync();

            if (!isDentist)
            {
                throw new Exception("Dentist cannot be identified!");
            }
        }

        private async Task ValidateIfDentistIsFree(int dentistId, DateTime startDate)
        {
            var existingReservation = await _db.Appointments.Where(x => x.DentistId == dentistId && x.StartTime == startDate).AnyAsync();

            if (existingReservation)
            {
                throw new Exception("The slot is not available for this dentist!");
            }
        }
        
    }
}
