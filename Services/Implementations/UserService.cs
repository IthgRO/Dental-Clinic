using Dental_Clinic.Dtos;
using Dental_Clinic.Enums;
using Infrastructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Services.Interfaces;
using Services.Models.User;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text.RegularExpressions;

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
        public ReminderType GetUserReminderType(int userId)
        {
            var user = _db.Users.FirstOrDefault(u => u.Id == userId);
            if (user == null)
            {
                throw new Exception($"User with ID {userId} not found.");
            }
            return user.ReminderType;
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
                        Timezone = a.Timezone

                    }).ToList()
                })
                .FirstOrDefaultAsync();
        }

        public async Task<LoginCodeDto> GetLoginCode(string email, string password)
        {
            var userFromDb = await _db.Users.Where(x => x.Email == email).FirstOrDefaultAsync();
            if (userFromDb is not null && BCrypt.Net.BCrypt.Verify(password, userFromDb.Password))
            {
                if (userFromDb.Role == Dental_Clinic.Enums.UserRole.Dentist || userFromDb.Role == Dental_Clinic.Enums.UserRole.Admin)
                {
                    var code = GenerateCode(10);

                    var codeFromDb = await _db.DoctorLoginAttempts
                        .Where(x => x.Email == email)
                        .FirstOrDefaultAsync();

                    if (codeFromDb is not null)
                    {
                        codeFromDb.Code = code;
                        codeFromDb.DateCreated = DateTime.UtcNow;
                    }

                    else
                    {
                        _db.DoctorLoginAttempts.Add(
                            new Infrastructure.Dtos.DoctorLoginAttemptDto
                            {
                                Email = email,
                                Code = code,
                                DateCreated = DateTime.UtcNow
                            });
                    }

                    await _db.SaveChangesAsync();
                    return new LoginCodeDto
                    {
                        Code = code,
                        Email = email,
                        FirstName = userFromDb.FirstName
                    };

                }
                else
                {
                    throw new Exception("2FA only needed for dentists and admins!");
                }
            }
            else
            {
                throw new Exception("Invalid credentials!");
            }

        }

        private string GenerateCode(int length)
        {
            var random = RandomNumberGenerator.GetBytes(100);
            var randomString = Convert.ToBase64String(random);
            string pattern = @"[^a-zA-Z0-9]";
            string result = Regex.Replace(randomString, pattern, "a");
            return result.Substring(0, length);
        }

        public async Task<LoginResultDto?> LoginWithCode(string email, string password, string code)
        {
            var userFromDb = await _db.Users.Where(x => x.Email == email).FirstOrDefaultAsync();
            if (userFromDb is not null && BCrypt.Net.BCrypt.Verify(password, userFromDb.Password))
            {
                if (userFromDb.Role is Dental_Clinic.Enums.UserRole.Dentist or Dental_Clinic.Enums.UserRole.Admin)
                {
                    var loginCodeFromDb = await _db.DoctorLoginAttempts
                        .Where(x => x.Email == email)
                        .FirstOrDefaultAsync();

                    int thresholdTimeInMinutes = -5;

                    if (loginCodeFromDb is not null && loginCodeFromDb.Code == code && loginCodeFromDb.DateCreated >= DateTime.UtcNow.AddMinutes(thresholdTimeInMinutes))
                    {
                        var jwt = await Login(new LoginUserDto
                        {
                            Email = email,
                            Password = password
                        });

                        _db.DoctorLoginAttempts.Remove(loginCodeFromDb);
                        await _db.SaveChangesAsync();

                        return jwt;
                    }
                    else
                    {
                        throw new Exception("Provided code is not valid!");
                    }
                }
                else
                {
                    throw new Exception("2FA only needed for dentists and admins!");
                }
            }
            else
            {
                throw new Exception("Invalid credentials!");
            }
        }

        public async Task ValidateAdminForSeeingDentistAppointmets(int adminId, int dentistId)
        {
            var admin = await _db.Users
                .Where(x => x.Id == adminId)
                .FirstOrDefaultAsync();

            var dentist = await _db.Users
                .Where(x => x.Id == dentistId)
                .FirstOrDefaultAsync();

            if(admin is null)
            {
                throw new Exception("User could not be found!");
            }

            if (dentist is null)
            {
                throw new Exception("Dentist could not be found!");
            }

            if (admin.Role != UserRole.Admin)
            {
                throw new Exception("This user is not an admin!");
            }

            if (dentist.Role != UserRole.Dentist)
            {
                throw new Exception("This user is not a dentist!");
            }

            if (admin.ClinicId != dentist.ClinicId)
            {
                throw new Exception("The admin and the dentist belong to different clinics!");
            }
        }
    }
}