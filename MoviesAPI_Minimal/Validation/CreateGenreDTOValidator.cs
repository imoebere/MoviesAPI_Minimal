using FluentValidation;
using MoviesAPI_Minimal.DTOs;
using MoviesAPI_Minimal.Repostories.Interface;

namespace MoviesAPI_Minimal.Validation
{
    public class CreateGenreDTOValidator : AbstractValidator<CreateGenreDTO>
    {
        public CreateGenreDTOValidator(IGenreRepository genreRepository, IHttpContextAccessor httpContext) 
        {
            var routeValueId = httpContext.HttpContext!.Request.RouteValues["id"];
            var id = 0;

            if (routeValueId is string routeValueIdString)
            {
                int.TryParse(routeValueIdString, out id );
            }

            RuleFor(p => p.Name)
                .NotEmpty().WithMessage(ValidationUtilities.NonEmptyMessage)
                .MaximumLength(150).WithMessage(ValidationUtilities.MaximumLengthMessage)
                .Must(ValidationUtilities.FirstLetterIsUpperCase).WithMessage(ValidationUtilities.FirstLettterIsUpperCaseMessage)
                .MustAsync(async (name, _) =>
                {
                    var exists = await genreRepository.Exists(id, name);
                    return !exists;
                }).WithMessage(g => $"A genre with the name {g.Name} already exists");
        }
    }
}
