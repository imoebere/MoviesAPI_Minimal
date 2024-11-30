
using AutoMapper;
using Microsoft.AspNetCore.Components.Forms;
using MoviesAPI_Minimal.Repostories.Interface;

namespace MoviesAPI_Minimal.Filters
{
    public class TestFilter : IEndpointFilter
    {
        public async ValueTask<object?> InvokeAsync(EndpointFilterInvocationContext context, EndpointFilterDelegate next)
        {

            //This is the code that will execute before the endpoint
            var param1 = context.Arguments.OfType<int>().FirstOrDefault();
                //(int)context.Arguments[0]!;
            var param2 = context.Arguments.OfType<IGenreRepository>().FirstOrDefault();
                //(IGenreRepository)context.Arguments[1]!;
            var param3 = context.Arguments.OfType<IMapper>().FirstOrDefault();

            var result = await next(context);
            //This is the code that will execute after the endpoint
            return result;
        }
    }
}
