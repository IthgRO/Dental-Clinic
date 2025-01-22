using FluentValidation;

namespace Dental_Clinic.Requests.User
{
    public class UpdateUserDataRequest
    {
        public required string Phone {  get; set; }
        public required string Email { get; set; }
    }

    public class UpdateUserDataRequestAbstractValidator : AbstractValidator<UpdateUserDataRequest>
    {
        public UpdateUserDataRequestAbstractValidator()
        {
            RuleFor(x => x.Email)
                .NotEmpty().WithMessage("Email is required")
                .EmailAddress().WithMessage("Invalid email format");

            RuleFor(x => x.Phone)
                .NotEmpty().WithMessage("Phone Number is required")
                .MaximumLength(15).WithMessage("Invalid phone format");
        }
    }
}
