using Dental_Clinic.Dtos;
using Services.Models.User;

namespace Services.Interfaces
{
    public interface IUserService
    {
        public Task RegisterUser(RegisterUserDto user); 

        public Task<string> Login(LoginUserDto user);

        public Task<DentistListDto> GetAvailableDentists();

        Task<UserDto?> GetUserByIdAsync(int userId);
    }
}
