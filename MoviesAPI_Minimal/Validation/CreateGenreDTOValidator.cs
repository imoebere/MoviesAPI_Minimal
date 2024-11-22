using FluentValidation;
using MoviesAPI_Minimal.DTOs;

namespace MoviesAPI_Minimal.Validation
{
    public class CreateGenreDTOValidator : AbstractValidator<CreateGenreDTO>
    {
        public CreateGenreDTOValidator() 
        {
            RuleFor(p => p.Name)
                .NotEmpty().WithMessage("The field {PropertyName} is required");
        }
    }
}
