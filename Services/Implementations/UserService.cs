using Dental_Clinic.Dtos;
using Infrastructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Services.Interfaces;
using Services.Models.User;
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

        public async Task<LoginResultDto> Login(LoginUserDto user)
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

                return new LoginResultDto
                {
                    Jwt = jwt,
                    FirstName = userFromDb.FirstName,
                    LastName = userFromDb.LastName,
                    Email = userFromDb.Email,
                    Phone = userFromDb.Phone,
                    Role = (int)userFromDb.Role
                };
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
                Password = hashedPassword,
                Role = user.Role,

            };
            if (await _db.Users.AnyAsync(u => u.Email == user.Email))
        {
            throw new Exception("Email already exists");
        }

            _db.Users.Add(newUser);
            await _db.SaveChangesAsync();
        }

        public async Task<DentistListDto> GetAvailableDentists()
        {
            var dentists = await _db.Users
                .Where(x => x.Role == Dental_Clinic.Enums.UserRole.Dentist)
                .Include(x => x.Clinic)
                .Include(x => x.Services)
                .ToListAsync();

            return new DentistListDto { Dentists = dentists };
        }
        public async Task<UserDto?> GetUserByIdAsync(int userId)
    {
        return await _db.Users
            .Where(u => u.Id == userId)
            .Select(u => new UserDto
            {
                Id = u.Id,
                Email = u.Email,
                FirstName = u.FirstName,
                LastName = u.LastName,
                Role = u.Role,
                Password = u.Password,
                ClinicId = u.ClinicId,
                Clinic = u.Clinic != null ? new ClinicDto
                {
                    Id = u.Clinic.Id,
                    Name = u.Clinic.Name,
                    Address = u.Clinic.Address,
                    City = u.Clinic.City,
                    Country = u.Clinic.Country,
                    Timezone = u.Clinic.Timezone,
                    Phone = u.Clinic.Phone,
                    Email = u.Clinic.Email,
                    IsActive = u.Clinic.IsActive,
                    CreatedAt = u.Clinic.CreatedAt,
                    UpdatedAt = u.Clinic.UpdatedAt
                    
                } : null,
                IsActive = u.IsActive,
                Timezone = u.Timezone,
                Phone = u.Phone,
                CreatedAt = u.CreatedAt,
                ModifiedAt = u.ModifiedAt,
                Services = u.Services.Select(s => new ServiceDto
            {
                Id = s.Id,
                UserId = s.UserId,
                User = s.User != null ? new UserDto
                {
                    Id = s.User.Id,
                    ClinicId = s.User.ClinicId,
                    Email = s.User.Email,
                    FirstName = s.User.FirstName,
                    LastName = s.User.LastName,
                    Phone = s.User.Phone,
                    Role = s.User.Role,
                    Timezone = s.User.Timezone,
                    IsActive = s.User.IsActive,
                    CreatedAt = s.User.CreatedAt,
                    ModifiedAt = s.User.ModifiedAt,
                    Password = s.User.Password
                } : null,
                    Name = s.Name,
                    Description = s.Description,
                    Duration = s.Duration,
                    Price = s.Price,
                    Currency = s.Currency,
                    Category = s.Category,
                    IsActive = s.IsActive,
                    CreatedAt = s.CreatedAt,
                    UpdatedAt = s.UpdatedAt
                }).ToList(),
                Appointments = u.Appointments.Select(a => new AppointmentDto
                {
                    Id = a.Id,
                    StartTime = a.StartTime,
                    EndTime = a.EndTime,
                    Status = a.Status,
                    Timezone =a.Timezone

                }).ToList()
            })
            .FirstOrDefaultAsync();
        }
    }
}
