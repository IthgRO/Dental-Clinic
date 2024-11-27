using Dental_Clinic.Dtos;
using Infrastructure;
using Services.Interfaces;
using Services.Models.User;

namespace Services.Implementations
{
    public class UserService : IUserService
    {

        private readonly ApplicationContext _db;

        public UserService(ApplicationContext db) 
        { 
            _db = db;
        }

        public async Task RegisterUser(RegisterUserDto user)
        {
            var newUser = new UserDto
            {   
                Email = user.Email,
                Phone = user.Phone,
                FirstName = user.FirstName,
                LastName = user.LastName,
                ClinicId = user.ClinicId,
                CreatedAt = DateTime.UtcNow,
                Timezone = user.Timezone,
                IsActive = true,
            };

            _db.Users.Add(newUser);
            await _db.SaveChangesAsync();
        }
    }
}
