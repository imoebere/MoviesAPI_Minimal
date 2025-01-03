﻿
using Dapper;
using Microsoft.Data.SqlClient;
using MoviesAPI_Minimal.DTOs;
using MoviesAPI_Minimal.Entities;
using MoviesAPI_Minimal.Repostories.Interface;
using System.Data;
using System.Net.Http;

namespace MoviesAPI_Minimal.Repostories
{
    public class MoviesRepository : IMoviesRepository
    {
        private readonly string connectionString;
        private readonly HttpContext httpContext;

        public MoviesRepository(IConfiguration configuration, IHttpContextAccessor contextAccessor)
        {
            connectionString = configuration.GetConnectionString("DefaultConnection")!;
            httpContext = contextAccessor.HttpContext!;
        }

        public async Task<int> Create(Movie movie)
        {
            using (var connection = new SqlConnection(connectionString))
            {
                var id = await connection.QuerySingleAsync<int>("Movie_Create", new
                { movie.Title, movie.Poster, movie.InTheaters, movie.ReleaseDate },
                commandType: CommandType.StoredProcedure);
                movie.Id = id;
                return id;
            }
        }

        public async Task<List<Movie>> GetAll(PaginationDTO pagination)
        {
            using (var connection = new SqlConnection(connectionString))
            {
                var movies = await connection.QueryAsync<Movie>("Movies_GetAll",
                   new { pagination.Page, pagination.RecordsPerPage }
                   , commandType: CommandType.StoredProcedure);

                var moviesCount = await connection.QuerySingleAsync<int>("Movies_Count",
                    commandType: CommandType.StoredProcedure);

                httpContext.Response.Headers.Append("toalAmountOfRecords", moviesCount.ToString());

                return movies.ToList();

            }
        }

        public async Task<Movie?> GetById(int id)
        {
            using (var connection = new SqlConnection(connectionString))
            {
                using(var multi = await connection.QueryMultipleAsync("Movies_GetById", new { id }))
                {
                    var movie = await multi.ReadFirstAsync<Movie>();
                    var comments = await multi.ReadAsync<Comment>();
                    var genres = await multi.ReadAsync<Genre>();
                    var actors = await multi.ReadAsync<ActorMovieDTO>();

                    movie.Comments = comments.ToList();

                    foreach (var genre in genres)
                    {
                        movie.GenresMovies.Add(new GenreMovie
                        {
                            GenreId = genre.Id,
                            Genre = genre
                        });
                    }

                    foreach (var actor in actors)
                    {
                        movie.ActorMovies.Add(new ActorMovie
                        {
                            ActorId = actor.Id,
                            Character = actor.Character,
                            Actor = new Actor { Name = actor.Name}
                        });
                    }
                    return movie;
                }
                //var movie = await connection.QueryFirstOrDefaultAsync<Movie>("Movies_GetById",
                   // new { id }, commandType: CommandType.StoredProcedure);
            }
        }

        public async Task<bool> Exist(int id)
        {
            using (var connection = new SqlConnection(connectionString))
            {
                var exist = await connection.QuerySingleAsync<bool>("Movies_Exist", new { id },
                    commandType: CommandType.StoredProcedure);

                return exist;
            }
        }

        public async Task Update(Movie movie)
        {
            using (var connection = new SqlConnection(connectionString))
            {
                await connection.ExecuteAsync("Movies_Update", new
                {
                    movie.Id,
                    movie.Title,
                    movie.ReleaseDate,
                    movie.Poster,
                    movie.InTheaters
                }, commandType: CommandType.StoredProcedure);

            }
        }

        public async Task Delete(int id)
        {
            using (var connection = new SqlConnection(connectionString))
            {
                await connection.ExecuteAsync("Movies_Delete", new { id },
                    commandType: CommandType.StoredProcedure);
            }
        }

        public async Task Assign(int id, List<int> genresIds)
        {
            var dt = new DataTable();
            dt.Columns.Add("Id", typeof(int));

            foreach(var genresId in genresIds)
            {
                dt.Rows.Add(genresId);
            }

            using (var connection = new SqlConnection(connectionString))
            {
                await connection.ExecuteAsync("Movies_AssignGenres", new {movieId = id, genresIds = dt },
                    commandType: CommandType.StoredProcedure);
            }
        }

        public async Task Assign (int Id, List<ActorMovie> actorMovies)
        {
            for(int i = 1; i < actorMovies.Count; i++)
            {
                actorMovies[i - 1].Order = i;
            }

            var dt = new DataTable();
            dt.Columns.Add("ActorId",  typeof(int));
            dt.Columns.Add("Character", typeof(string));
            dt.Columns.Add("Order", typeof(int));

            foreach (var actorMovie in actorMovies)
            {
                dt.Rows.Add(actorMovie.ActorId, actorMovie.Character, actorMovie.Order);
            }

            using ( var connection = new SqlConnection(connectionString))
            {
                await connection.ExecuteAsync("Movies_AssignActors", new { movieId = Id, actors = dt },
                    commandType: CommandType.StoredProcedure);
            }
        }
    }
}
