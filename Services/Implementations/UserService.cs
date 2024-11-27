using Dental_Clinic.Dtos;
using Infrastructure;
using Services.Interfaces;
using Services.Models.User;
using Microsoft.EntityFrameworkCore;

using BCrypt.Net;

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
            string hashedPassword = BCrypt.Net.BCrypt.HashPassword(user.Password);
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
                Password = hashedPassword

            };
            if (await _db.Users.AnyAsync(u => u.Email == user.Email))
        {
            throw new Exception("Email already exists");
        }

            _db.Users.Add(newUser);
            await _db.SaveChangesAsync();
        }
    }
}
