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
                .MaximumLength(20)
                .WithMessage("First name cannot be longer than 20 characters! ");

            RuleFor(x => x.LastName)
            .NotEmpty().WithMessage("Last name is required")
            .MaximumLength(20).WithMessage("Last name cannot exceed 20 characters");

            RuleFor(x => x.Email)
                .NotEmpty().WithMessage("Email is required")
                .EmailAddress().WithMessage("Invalid email format");
            
            RuleFor(x => x.Phone)
                .NotEmpty().WithMessage("Phone Number is required")
                .MaximumLength(15).WithMessage("Invalid phone format");

            RuleFor(x => x.Password)
                .NotEmpty().WithMessage("Password is required")
                .MinimumLength(8).WithMessage("Password must be at least 8 characters")
                .Matches("[A-Z]").WithMessage("Password must contain an uppercase letter")
                .Matches("[a-z]").WithMessage("Password must contain a lowercase letter")
                .Matches("[0-9]").WithMessage("Password must contain a number");  

            RuleFor(x => x.Role)
                .IsInEnum()
                .WithMessage("Role must be one of the predefined values: Patient, Dentist, Admin");   
        }
        

    }
}
