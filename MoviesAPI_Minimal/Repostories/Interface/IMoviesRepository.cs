using MoviesAPI_Minimal.DTOs;
using MoviesAPI_Minimal.Entities;

namespace MoviesAPI_Minimal.Repostories.Interface
{
    public interface IMoviesRepository
    {
        Task Assign(int id, List<int> genresIds);
        Task Assign(int Id, List<ActorMovie> actorMovies);
        Task<int> Create(Movie movie);
        Task Delete(int id);
        Task<bool> Exist(int id);
        Task<List<Movie>> GetAll(PaginationDTO pagination);
        Task<Movie?> GetById(int id);
        Task Update(Movie movie);
        Task<List<Movie>> Filter(MoviesFilterDTO moviesFilterDTO);
    }
}