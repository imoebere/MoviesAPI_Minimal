using FluentValidation;
using MoviesAPI_Minimal.DTOs;

namespace MoviesAPI_Minimal.Validation
{
    public class CreateActorDTOValidator : AbstractValidator<CreateActorDTO>
    {
        public CreateActorDTOValidator()
        {
            RuleFor(p => p.Name)
             .NotEmpty().WithMessage("The field {PropertyName} is required")
             .MaximumLength(150).WithMessage("The field {PropertyName} should be less than {MaxLength} characters");

            var minimumDate = new DateTime(1900, 1, 1);
            RuleFor(p => p.DateOfBirth).GreaterThanOrEqualTo(minimumDate)
                .WithMessage("The field {PropertyName} should be greater than " + minimumDate.ToString("yyyy-MM-dd"));

        }
    }
}
