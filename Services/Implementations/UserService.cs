using Dental_Clinic.Dtos;
using Infrastructure;
using Services.Interfaces;
using Services.Models.User;
using Microsoft.EntityFrameworkCore;

using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace Services.Implementations
{
    public class UserService : IUserService
    {

        private readonly ApplicationContext _db;

        public UserService(ApplicationContext db) 
        { 
            _db = db;
        }

        public async Task<string> Login(LoginUserDto user)
        {
            var userFromDb = await _db.Users.Where(x => x.Email == user.Email).FirstOrDefaultAsync();
            if (userFromDb is not null && BCrypt.Net.BCrypt.Verify(user.Password, userFromDb.Password))
            {
                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.Actor, userFromDb.Id.ToString()),
                    new Claim(ClaimTypes.Role, userFromDb.Role.ToString())
                };

                var key = new SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes("abdivbeiuvbiaubviubviwnvjwnviubwnifbwuybvuwbfubvuebvuybwbfbfgwevgyv4354366erbvuwybvyuwbvueybve"));

                var cred = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);

                var token = new JwtSecurityToken(
                    claims: claims,
                    expires: DateTime.UtcNow.AddYears(1),
                    signingCredentials: cred);

                var jwt = new JwtSecurityTokenHandler().WriteToken(token);
                return jwt;
            }
            else
            {
                throw new Exception("Invalid credentials!");
            }
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
