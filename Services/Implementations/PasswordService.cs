using Infrastructure;
using Infrastructure.Dtos;
using Microsoft.EntityFrameworkCore;
using Services.Interfaces;
using Services.Models.User;
using System.Security.Cryptography;
using System.Text.RegularExpressions;

namespace Services.Implementations
{
    public class PasswordService: IPasswordService
    {
        private readonly ApplicationContext _context;

        public PasswordService(ApplicationContext context) 
        {
            _context = context;
        }

        public async Task ChangeForgottenPassword(string email, string code, string newPassword)
        {
            var passwordChangeAttemptFromDb = await _context.PasswordChangeAttempts
                .Where(x => x.Email == email)
                .FirstOrDefaultAsync() ?? throw new Exception("No password change attempt was found!");

            var userFromDb = await _context.Users
                .Where(x => x.Email == email)
                .FirstOrDefaultAsync() ?? throw new Exception("No user with this email could be found!");

            int treshold_in_minutes = 5;

            if (passwordChangeAttemptFromDb.DateCreated >= DateTime.UtcNow.AddMinutes(treshold_in_minutes))
            {
                throw new Exception("The code has alreay expired!");
            }

            if (passwordChangeAttemptFromDb.Code != code)
            {
                throw new Exception("The code you provided is incorrect!");
            }

            userFromDb.Password = BCrypt.Net.BCrypt.HashPassword(newPassword);
            _context.PasswordChangeAttempts.Remove(passwordChangeAttemptFromDb);

            await _context.SaveChangesAsync();
        }

        public async Task<PasswordChangeAttemptDto> GetPasswordChangeCode(string email)
        {
            var userFromDb = await _context.Users
                .Where(u => u.Email == email)
                .FirstOrDefaultAsync() ?? throw new Exception("User with this email could not be found");

            var attemptFromDb = await _context.PasswordChangeAttempts
                .Where(u => u.Email == email)
                .FirstOrDefaultAsync();

            var code = GenerateCode(10);

            if (attemptFromDb == null)
            {
                await _context.PasswordChangeAttempts
                    .AddAsync(new PasswordChangeAttempt
                    {
                        Email = email,
                        Code = code,
                        DateCreated = DateTime.UtcNow,                       
                    });
            }
            else
            {
                attemptFromDb.Code = code;
                attemptFromDb.DateCreated = DateTime.UtcNow;
            }
            await _context.SaveChangesAsync();

            return new PasswordChangeAttemptDto
            {
                Code = code,
                Email = email,
                FirstName = userFromDb.FirstName,
            };
        }

        private string GenerateCode(int length)
        {
            var random = RandomNumberGenerator.GetBytes(100);
            var randomString = Convert.ToBase64String(random);
            string pattern = @"[^a-zA-Z0-9]";
            string result = Regex.Replace(randomString, pattern, "a");
            return result.Substring(0, length);
        }
    }
}
