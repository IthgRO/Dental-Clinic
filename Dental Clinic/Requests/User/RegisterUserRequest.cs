using Dental_Clinic.Enums;
using FluentValidation;
using System.Text.Json.Serialization;

namespace Dental_Clinic.Requests.User
{
    public class RegisterUserRequest
    {
        public int Id { get; set; }

        public int? ClinicId { get; set; }

        public required string Email { get; set; }

        public required string FirstName { get; init; }

        public required string LastName { get; init; }

        public required string Phone { get; set; }

        [JsonConverter(typeof(JsonStringEnumConverter<UserRole>))]
        public UserRole Role { get; set; }

        public string? Timezone { get; set; }
        public required string Password { get; set; }
    }

    public class RegisterUserRequestAbstractValidator: AbstractValidator<RegisterUserRequest>
    {
        public RegisterUserRequestAbstractValidator() 
        {
            RuleFor(x => x.FirstName)
                .MaximumLength(10)
                .WithMessage("First name cannot be longer than 10 characters! ");     
        }
    }
}
