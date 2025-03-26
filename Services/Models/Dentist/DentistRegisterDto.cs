﻿
namespace Services.Models.Dentist
{
    public class DentistRegisterDto
    {
        public required string Email { get; set; }

        public required string FirstName { get; init; }

        public required string LastName { get; init; }

        public required string Phone { get; set; }

        public string? Timezone { get; set; }
        public required string Password { get; set; }
    }
}
