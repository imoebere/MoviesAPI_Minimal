using AutoMapper;
using FluentValidation;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.OutputCaching;
using MoviesAPI_Minimal.DTOs;
using MoviesAPI_Minimal.Entities;
using MoviesAPI_Minimal.Filters;
using MoviesAPI_Minimal.Repostories.Interface;

namespace MoviesAPI_Minimal.Endpoints
{
    public static class GenresEndpoints
    {
        public static RouteGroupBuilder MapGenres(this RouteGroupBuilder group)
        {
            group.MapGet("/", GetGenres).
                CacheOutput(c => c.Expire(TimeSpan.FromSeconds(60)).Tag("genres-get"))
                .RequireAuthorization();
            group.MapGet("/{id:int}", GetById);
            group.MapPost("/", Create)
                .AddEndpointFilter<ValidationFilter<CreateGenreDTO>>()
                .RequireAuthorization("isadmin");
            group.MapPut("/{id:int}", Update)
                .AddEndpointFilter<ValidationFilter<CreateGenreDTO>>()
                .RequireAuthorization("isadmin");
            group.MapDelete("/{id:int}", Delete).RequireAuthorization("isadmin");
            return group;
        }


        static async Task<Ok<List<GenreDTO>>> GetGenres(IGenreRepository genreRepository, IMapper mapper)
        {
            var genres = await genreRepository.GetAll();
            var genreDTOs = mapper.Map<List<GenreDTO>>(genres);
                //genres.Select(x => new GenreDTO { Id = x.Id, Name = x.Name}).ToList();
            return TypedResults.Ok(genreDTOs);

        }

        static async Task<Results<Ok<GenreDTO>, NotFound>> GetById(int id, IGenreRepository genreRepository, 
                        IMapper mapper)
        {
            var genres = await genreRepository.GetById(id);

            if (genres is null)
            {
                return TypedResults.NotFound();
            }
            var genreDTO = mapper.Map<GenreDTO>(genres);
            //new GenreDTO{Id = genres.Id,Name = genres.Name};
            return TypedResults.Ok(genreDTO);
        }

        static async Task<Created<GenreDTO>> Create(CreateGenreDTO createGenresDTO, IGenreRepository genresRepository,
            IOutputCacheStore outputCacheStore, IMapper mapper)
        {
            /*var validateResult = await validator.ValidateAsync(createGenresDTO);

            if (!validateResult.IsValid) 
            {
                return TypedResults.ValidationProblem(validateResult.ToDictionary());
            }*/

            var genres = mapper.Map<Genre>(createGenresDTO); 
            await genresRepository.Create(genres);
            await outputCacheStore.EvictByTagAsync("genres-get", default);

            var genreDTO = mapper.Map<GenreDTO>(genres);
            return TypedResults.Created($"/genres/{genres.Id}", genreDTO);
        }

        static async Task<Results<NotFound, NoContent>> Update(int id, CreateGenreDTO createGenresDTO, 
           IGenreRepository genreRepository, IOutputCacheStore outputCacheStore, IMapper mapper)
        {
           /* var validationResult = await validator.ValidateAsync(createGenresDTO);

            if (!validationResult.IsValid)
            {
                return TypedResults.ValidationProblem(validationResult.ToDictionary());
            }*/


            var exists = await genreRepository.Exists(id);

            if (!exists)
            {
                return TypedResults.NotFound();
            }
            var genres = mapper.Map<Genre>(createGenresDTO);
            genres.Id = id;
            
            //new Genre{Id = id,Name = createGenresDTO.Name,};
            await genreRepository.Update(genres);
            await outputCacheStore.EvictByTagAsync("genres-get", default);
            return TypedResults.NoContent();
        }

        static async Task<Results<NoContent, NotFound>> Delete(int id, IGenreRepository genreRepository,
            IOutputCacheStore outputCacheStore)
        {
            var exists = await genreRepository.Exists(id);

            if (!exists)
            {
                return TypedResults.NotFound();
            }
            await genreRepository.Delete(id);
            await outputCacheStore.EvictByTagAsync("genres-get", default);
            return TypedResults.NoContent();
        }
    }
}
