using FluentValidation;
using System.ComponentModel.DataAnnotations;

namespace Dental_Clinic.Requests.Service
{
    public class AddServiceToDentistRequest
    {
        public required string Name { get; set; }
        public required string Category { get; set; }

        [Range(0, int.MaxValue, ErrorMessage = "The value must be greater than 0")]
        public int Duration { get; set; }
    }

    public class AddServiceToDentistRequestAbstractValidator : AbstractValidator<AddServiceToDentistRequest>
    {
        public AddServiceToDentistRequestAbstractValidator() 
        {
            RuleFor(x => x.Duration)
                .GreaterThan(0)
                .WithMessage("Service duration cannot be empty!");

            RuleFor(x => x.Name)
                .NotEmpty()
                .WithMessage("Service name cannot be empty!");

            RuleFor(x => x.Category)
                .NotEmpty()
                .WithMessage("Category cannot be empty!");
        }
    }
}
