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
                .NotEmpty().WithMessage("The field {PropertyName} is required")
                .MaximumLength(150).WithMessage("The field {PropertyName} should be less than {MaxLength} characters")
                .Must(FirstLetterIsUpperCase).WithMessage("The field {PropertyName} should start with uppercase")
                .MustAsync(async (name, _) =>
                {
                    var exists = await genreRepository.Exists(id, name);
                    return !exists;
                }).WithMessage(g => $"A genre with the name {g.Name} already exists");
        }

        private bool FirstLetterIsUpperCase(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                return false;
            }

            var firstLetter = value[0].ToString();
            return firstLetter == firstLetter.ToUpper();
        }
    }
}
