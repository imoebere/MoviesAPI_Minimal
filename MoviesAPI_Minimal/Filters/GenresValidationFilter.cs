
using FluentValidation;
using MoviesAPI_Minimal.DTOs;

namespace MoviesAPI_Minimal.Filters
{
    public class GenresValidationFilter : IEndpointFilter
    {
        public async ValueTask<object?> InvokeAsync(EndpointFilterInvocationContext context, EndpointFilterDelegate next)
        {
            var validator = context.HttpContext.RequestServices.GetService<IValidator<CreateGenreDTO>>();

            if (validator is null)
            { 
                return await next(context);
            }

            var obj = context.Arguments.OfType<CreateGenreDTO>().FirstOrDefault();

            if (obj is null)
            {
                return Results.Problem("The Object to validate could not be found");
            }

            var validationResult = await validator.ValidateAsync(obj);

            if (!validationResult.IsValid) 
            {
                return Results.ValidationProblem(validationResult.ToDictionary());
            }

            return await next(context);
        }
    }
}
