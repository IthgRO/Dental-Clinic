using Services.Models.Schedule;

namespace Services.Interfaces
{
    public interface IScheduleService
    {
        public Task AddSchedule(int dentistId, TimeSpan startTime, TimeSpan endTime, DayOfWeek dayOfWeek);

        public Task DeleteSchedule(int dentistId, int scheduleId);

        public Task<List<ScheduleDto>> GetDentistSchedules(int dentistId);
    }
}
