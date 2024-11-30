using FluentValidation;
using MoviesAPI_Minimal.DTOs;

namespace MoviesAPI_Minimal.Validation
{
    public class CreateMovieDTOValidator : AbstractValidator<CreateMovieDTO>
    {
        public CreateMovieDTOValidator()
        {
            RuleFor(x => x.Title).NotEmpty().WithMessage(ValidationUtilities.NonEmptyMessage)
                .MaximumLength(250).WithMessage(ValidationUtilities.MaximumLengthMessage);

        }
    }
}
