using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;
using MoviesAPI_Minimal.DTOs;

namespace MoviesAPI_Minimal.Utilities
{
    public static class SwaggerExtensions
    {
        public static TBuilder AddPaginationParameters<TBuilder>(this TBuilder builder) 
            where TBuilder : IEndpointConventionBuilder
        {
            return builder.WithOpenApi(options =>
            {
                options.Parameters.Add(new OpenApiParameter
                {
                    Name = "Page",
                    In = ParameterLocation.Query,
                    Schema = new OpenApiSchema
                    {
                        Type = "integer",
                        Default = new OpenApiInteger(PaginationDTO.pageInitialValue)
                    }
                });

                options.Parameters.Add(new OpenApiParameter
                {
                    Name = "RecordsPerPage",
                    In = ParameterLocation.Query,
                    Schema = new OpenApiSchema
                    {
                        Type = "integer",
                        Default = new OpenApiInteger(PaginationDTO.recordsPerPageInitialValue)
                    }
                });
                return options;

            });

        }
    }
}
