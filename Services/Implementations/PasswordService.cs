﻿using Infrastructure;
using Infrastructure.Dtos;
using Microsoft.EntityFrameworkCore;
using Services.Interfaces;
using Services.Models.User;
using System.CodeDom.Compiler;
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

        public Task ChangeForgottenPassword(string code, string newPassword)
        {
            throw new NotImplementedException();
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
