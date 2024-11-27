using Services.Models.User;

namespace Services.Interfaces
{
    public interface IUserService
    {
        public Task RegisterUser(RegisterUserDto user); 
    }
}
