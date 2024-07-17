using AutoMapper;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OutputCaching;
using MoviesAPI_Minimal.DTOs;
using MoviesAPI_Minimal.Entities;
using MoviesAPI_Minimal.Repostories.Interface;
using MoviesAPI_Minimal.Services;

namespace MoviesAPI_Minimal.Endpoints
{
    public static class MoviesEndpoints
    {
        private readonly static string container = "movies";
        public static RouteGroupBuilder MapMovies(this RouteGroupBuilder group) 
        {
            group.MapPost("/", Create).DisableAntiforgery();
            return group;
        }

        static async Task<Created<MovieDTO>> Create([FromForm] CreateMovieDTO createMovieDTO,
            IFileStorage fileStorage, IOutputCacheStore outputCacheStore, IMapper mapper, 
            IMoviesRepository moviesRepository)
        {
            var movie = mapper.Map<Movie>(createMovieDTO);

            if (createMovieDTO.Poster != null)
            {
                var url = await fileStorage.Store(container, createMovieDTO.Poster);
                movie.Poster = url;
            }

            var id = await moviesRepository.Create(movie);
            await outputCacheStore.EvictByTagAsync("movies-get", default);
            var movieDTO = mapper.Map<MovieDTO>(movie);
            return TypedResults.Created($"movies/{id}", movieDTO);


        }

    }
}
