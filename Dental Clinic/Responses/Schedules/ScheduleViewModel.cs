namespace Dental_Clinic.Responses.Schedules
{
    public class ScheduleViewModel
    {
        public DayOfWeek DayOfWeek { get; set; }
        public TimeSpan StartTime { get; set; }
        public TimeSpan EndTime { get; set; }
    }
}