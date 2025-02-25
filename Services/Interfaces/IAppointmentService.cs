using Services.Models.Reservation;

namespace Services.Interfaces
{
    public interface IAppointmentService
    {
        public Task<List<FreeSlot>> GetAvailableTimeSlots(int dentistId, DateTime startDate, DateTime endDate);

        public Task<Dental_Clinic.Dtos.AppointmentDto> BookAppointment(int userId, int dentistId, int serviceId, int clinicId, DateTime startDate);

        public Task<List<AppointmentDto>> GetMyAppointments(int userId);

        public Task<List<DentistAppointmentDto>> GetDentistAppointments(int dentistId);

        public Task CancelAppointment(int userId, int appointmentId);

        public Task ConfirmAppointment(int userId, int appointmentId);

        public Task UpdateAppointment(int userId, int appointmentId, DateTime newDate);
        Task<Dental_Clinic.Dtos.AppointmentDto> GetAppointmentByIdAsync(int appointmentId);
    }
}
