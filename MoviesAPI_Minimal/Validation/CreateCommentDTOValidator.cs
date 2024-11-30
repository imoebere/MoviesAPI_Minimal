using FluentValidation;
using MoviesAPI_Minimal.DTOs;

namespace MoviesAPI_Minimal.Validation
{
    public class CreateCommentDTOValidator : AbstractValidator<CreateCommentDTO>
    {
        public CreateCommentDTOValidator()
        {
            RuleFor(x => x.Body).NotEmpty().WithMessage(ValidationUtilities.NonEmptyMessage);
        }
    }
}
