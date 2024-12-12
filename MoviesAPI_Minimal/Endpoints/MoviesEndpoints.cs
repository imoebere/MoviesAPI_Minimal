using AutoMapper;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OutputCaching;
using MoviesAPI_Minimal.DTOs;
using MoviesAPI_Minimal.Entities;
using MoviesAPI_Minimal.Filters;
using MoviesAPI_Minimal.Repostories;
using MoviesAPI_Minimal.Repostories.Interface;
using MoviesAPI_Minimal.Services.Interface;

namespace MoviesAPI_Minimal.Endpoints
{
    public static class MoviesEndpoints
    {
        private readonly static string container = "movies";
        public static RouteGroupBuilder MapMovies(this RouteGroupBuilder group)
        {
            group.MapGet("/", GetAll)
                .CacheOutput(c => c.Expire(TimeSpan.FromMinutes(1)).Tag("movies-get"));
            group.MapGet("/{id:int}", GetById);
            group.MapPost("/", Create).DisableAntiforgery()
                .AddEndpointFilter<ValidationFilter<CreateMovieDTO>>().RequireAuthorization("isadmin");

            group.MapPut("/{id:int}", Update).DisableAntiforgery()
                .AddEndpointFilter<ValidationFilter<CreateMovieDTO>>().RequireAuthorization("isadmin");
            group.MapDelete("/{id:int}", Delete).RequireAuthorization("isadmin");
            group.MapPost("/{id:int}/assignGenres", AssignGenres).RequireAuthorization("isadmin");
            group.MapPost("/{id:int}/assignActors", AssignActors).RequireAuthorization("isadmin");
            return group;
        }

        static async Task<Ok<List<MovieDTO>>> GetAll(IMoviesRepository moviesRepository, IMapper mapper,
            int page = 1, int recordsPerPage = 10)
        {
            var pagination = new PaginationDTO { Page = page, RecordsPerPage = recordsPerPage };
            var movies = await moviesRepository.GetAll(pagination);
            var movieDTO = mapper.Map<List<MovieDTO>>(movies);
            return TypedResults.Ok(movieDTO);
        }

        static async Task<Results<Ok<MovieDTO>, NotFound>> GetById(int id, IMoviesRepository moviesRepository,
            IMapper mapper)
        {
            var movie = await moviesRepository.GetById(id);

            if (movie is null)
            {
                return TypedResults.NotFound();
            }

            var movieDTO = mapper.Map<MovieDTO>(movie);
            return TypedResults.Ok(movieDTO);
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

        static async Task<Results<NoContent, NotFound>> Update(int id, [FromForm] CreateMovieDTO createMovieDTO,
            IFileStorage fileStorage, IMoviesRepository moviesRepository, IOutputCacheStore outputCacheStore,
            IMapper mapper)
        {
            var movieDB = await moviesRepository.GetById(id);

            if (movieDB is null)
            {
                return TypedResults.NotFound();
            }

            var movieForUpdate = mapper.Map<Movie>(createMovieDTO);
            movieForUpdate.Id = id;
            movieForUpdate.Poster = movieDB.Poster;

            if (createMovieDTO.Poster is not null)
            {
                var url = await fileStorage.Edit(movieForUpdate.Poster, container, createMovieDTO.Poster);
                movieForUpdate.Poster = url;
            }

            await moviesRepository.Update(movieForUpdate);
            await outputCacheStore.EvictByTagAsync("movies-get", default);
            return TypedResults.NoContent();
        }

        static async Task<Results<NoContent, NotFound>> Delete(int id, IFileStorage fileStorage,
           IMoviesRepository moviesRepository, IOutputCacheStore outputCacheStore)
        {
            var movieDB = await moviesRepository.GetById(id);

            if (movieDB is null)
            {
                return TypedResults.NotFound();
            }

            await moviesRepository.Delete(id);
            await fileStorage.Delete(movieDB.Poster, container);
            await outputCacheStore.EvictByTagAsync("movies-get", default);
            return TypedResults.NoContent();

        }

        static async Task<Results<NoContent, NotFound, BadRequest<string>>> AssignGenres(int id,
            List<int> genresIds, IMoviesRepository moviesRepository, IGenreRepository genreRepository)
        {
            if (!await moviesRepository.Exist(id))
            {
                return TypedResults.NoContent();
            }

            var existingGenres = new List<int>();

            if (genresIds.Count != 0)
            {
                existingGenres = await genreRepository.Exists(genresIds);
            }

            if (genresIds.Count != existingGenres.Count)
            {
                var nonExistingGenres = genresIds.Except(existingGenres);

                var nonExistingGenresCSV = string.Join(",", nonExistingGenres);

                return TypedResults.BadRequest($"The genres of id {nonExistingGenresCSV} does not exist.");
            }

            await moviesRepository.Assign(id, genresIds);
            return TypedResults.NoContent();

        }

        static async Task<Results<NoContent, NotFound, BadRequest<string>>> AssignActors (int id,
            List<AssignActorMovieDTO> actorDTO, IMoviesRepository moviesRepository,
            IActorsRepository actorsRepository, IMapper mapper)
        {
            if (!await moviesRepository.Exist(id))
            {
                return TypedResults.NoContent();
            }

            var existingActors = new List<int>();
            var actorsIds = actorDTO.Select(a => a.ActorId).ToList();

            if (actorsIds.Count != 0)
            {
                existingActors = await actorsRepository.Exists(actorsIds);
            }

            if (existingActors.Count != actorsIds.Count)
            {
                var nonExistingActors = actorsIds.Except(existingActors);
                var nonExistingActorsCSV = string.Join(",", nonExistingActors);
                return TypedResults.BadRequest($"The actors of Id {nonExistingActorsCSV} do not exists");
            }

            var actors = mapper.Map<List<ActorMovie>>(actorDTO);
            await moviesRepository.Assign(id, actors);
            return TypedResults.NoContent();
        }
    }
}
