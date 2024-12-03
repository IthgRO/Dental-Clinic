using Services.Models.Reservation;

namespace Services.Interfaces
{
    public interface IAppointmentService
    {
        public Task<List<FreeSlot>> GetAvailableTimeSlots(int dentistId, DateTime startDate, DateTime endDate);

        public Task BookAppointment(int userId, int dentistId, int serviceId, int clinicId, DateTime startDate);
    }
}
