﻿using Dental_Clinic.Enums;

namespace Services.Models.User
{
    public class RegisterUserDto
    {
        public int Id { get; set; }

        public required string Email { get; set; }

        public required string FirstName { get; init; }

        public required string LastName { get; init; }

        public required string Phone { get; set; }

        public string? Timezone { get; set; }

        public required string Password { get; set; }
    }
}
