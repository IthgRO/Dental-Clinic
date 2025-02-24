using Dental_Clinic.Dtos;
using Services.Models.User;
using Dental_Clinic.Enums;

namespace Services.Interfaces
{
    public interface IUserService
    {
        public Task RegisterUser(RegisterUserDto user); 

        public Task<LoginResultDto> Login(LoginUserDto user);

        public Task<LoginResultDto?> LoginWithCode(string email, string password, string code);

        public Task<DentistListDto> GetAvailableDentists();

        Task<UserDto?> GetUserByIdAsync(int userId);

        public Task<LoginCodeDto> GetLoginCode(string email, string password);

        ReminderType GetUserReminderType(int userId);

        public Task ValidateAdminForSeeingDentistAppointmets(int adminId, int dentistId);
    }
}
